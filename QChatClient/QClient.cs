using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin;

namespace QChatClient
{
    class QClient
    {
        public List<IFriendInfo> friends;
        public List<IGroupInfo> groups;
        public List<Conversation> conversations;
        public MiraiHttpSession session;
        public Conversation current;
        public EventHandler handler;
        public int lastpos = 0;

        public QClient(string addr, int port, string passwd, long account)
        {
            MiraiHttpSessionOptions options = new MiraiHttpSessionOptions(addr, port, passwd);
            session = new MiraiHttpSession();
            handler = new EventHandler(this);
            conversations = new List<Conversation>();
            session.ConnectAsync(options, account).Wait();
            friends = session.GetFriendListAsync().Result.ToList();
            groups = session.GetGroupListAsync().Result.ToList();
            session.AddPlugin(handler);
        }

        public Conversation startConversation(long peerid, string peername, short type, long tgroup = -1)
        {
            foreach (Conversation cov in conversations)
            {
                if (cov.peerid == peerid && cov.type == type) return cov;
            }
            Conversation conver = new Conversation()
            { peername = peername, peerid = peerid, type = type, tgroup = tgroup, session = session };
            if (!conversations.Contains(conver)) conversations.Add(conver);
            return conver;
        }

        public void setCurrent(Conversation conver)
        {
            if (!conversations.Contains(conver)) conversations.Add(conver);
            current = conver;
            refreshCurrent();
        }

        public void chainReceived(IBaseInfo sender, IMessageBase[] msg, short type)
        {
            foreach (Conversation conv in conversations)
            {
                if (conv.peerid == sender.Id)
                {
                    conv.messages.Add(new KeyValuePair<IBaseInfo, IMessageBase[]>(sender, msg));
                    if (conv.Equals(current)) addonRefresh();
                    return;
                }
            }

            //没有符合的Conversation，创建。
            Conversation cv;
            if (type == 0)//好友私聊
                cv = startConversation(sender.Id, sender.Name, type);
            else//通过群聊临时聊天
                cv = startConversation(sender.Id, sender.Name, type, ((IGroupMemberInfo)sender).Group.Id);
            cv.messages.Add(new KeyValuePair<IBaseInfo, IMessageBase[]>(sender, msg));
        }

        public void chainReceived(IGroupInfo group, IGroupMemberInfo sender, IMessageBase[] msg)
        {
            foreach (Conversation conv in conversations)
            {
                if (conv.peerid == group.Id)
                {
                    conv.messages.Add(new KeyValuePair<IBaseInfo, IMessageBase[]>(sender, msg));
                    if (conv.Equals(current)) addonRefresh();
                    return;
                }
            }

            //没有符合的Conversation，创建。
            startConversation(sender.Group.Id, sender.Group.Name, 1).messages.Add(new KeyValuePair<IBaseInfo, IMessageBase[]>(sender, msg));
        }

        public void refreshCurrent()
        {
            lock ("ConsoleOp")
            {
                lastpos = -1;
                Program.Clear();
                Console.BufferWidth = Console.WindowWidth;
                Console.BufferHeight = Console.WindowHeight;
                Console.Title = Program.TITLE + current.peername + "#" + current.peerid;
                foreach (KeyValuePair<IBaseInfo, IMessageBase[]> msg in current.messages)
                {
                    string name = msg.Key.Name;
                    string id = msg.Key.Id.ToString();
                    Program.WriteLine("[#" + id + "] " + name + " ：\n");
                    printChain(msg.Value);
                    Program.WriteLine("\n----------\n");
                    lastpos++;
                }
                //Console.SetCursorPosition(2, Console.BufferHeight - 1);
            }
        }

        public void addonRefresh()
        {
            lock ("ConsoleOp")
            {
                Console.Title = Program.TITLE + current.peername + "#" + current.peerid;
                int i = -1;
                foreach (KeyValuePair<IBaseInfo, IMessageBase[]> msg in current.messages)
                {
                    i++;
                    if (i <= lastpos) continue;
                    string name = msg.Key.Name;
                    string id = msg.Key.Id.ToString();
                    Program.WriteLine("[#" + id + "] " + name + " ：\n");
                    printChain(msg.Value);
                    Program.WriteLine("\n----------\n");
                }
                //Console.SetCursorPosition(0, Console.BufferHeight - 1);
                //Console.Write(">>");
                if (i > lastpos) lastpos = i;
            }
        }

        public string getQName(long qq)
        {
            foreach (IFriendInfo i in friends)
            {
                if (i.Id == qq) return i.Name;
            }
            return null;
        }

        public string getGName(long group)
        {
            foreach (IGroupInfo i in groups)
            {
                if (i.Id == group) return i.Name;
            }
            return null;
        }

        public string getGMemberName(long group, long qq)
        {
            try
            {
                return session.GetGroupMemberInfoAsync(qq, group).Result.Name;
            }
            catch
            {
                return null;
            }
        }

        public void printChain(IMessageBase[] chain)
        {
            foreach (IMessageBase mmsg in chain)
                switch (mmsg.Type)
                {
                    case "Source":

                        Program.WriteLine("INFO >#" + ((SourceMessage)mmsg).Id + " @ " + ((SourceMessage)mmsg).Time.ToString());
                        break;
                    case "Plain":
                        Program.WriteLine("text >" + ((PlainMessage)mmsg).Message);
                        break;
                    case "Image":
                    case "FlashImage":

                        Program.WriteLine("pic >" + ((CommonImageMessage)mmsg).Url);
                        break;
                    case "Voice":

                        Program.WriteLine("pic >" + ((VoiceMessage)mmsg).Url);
                        break;
                    case "Poke":

                        Program.WriteLine("poke >" + ((PokeMessage)mmsg).Name);
                        break;
                    case "At":
                        if (((AtMessage)mmsg).Target == session.QQNumber)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }

                        Program.WriteLine("at >@" + ((AtMessage)mmsg).Display + " #" + ((AtMessage)mmsg).Target);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case "AtAll":

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Program.WriteLine("atAll >@全体成员");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case "Quote":
                        QuoteMessage m = (QuoteMessage)mmsg;
                        Console.BufferHeight += 2;
                        Program.WriteLine("quote >#" + m.Id + "\n“");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        printChain(m.OriginChain);
                        Console.ForegroundColor = ConsoleColor.Gray;

                        Program.WriteLine("quote >”");
                        break;
                    case "Face":
                        FaceMessage f = (FaceMessage)mmsg;

                        Program.WriteLine("face >[" + f.Name + "] #" + f.Id);
                        break;
                    default:

                        Program.WriteLine(mmsg.Type + " >[无法在控制台环境展示该消息]");
                        break;
                }
        }
    }
}
