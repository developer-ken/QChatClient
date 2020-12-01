using Mirai_CSharp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QChatClient
{
    class CommandHandler
    {
        public QClient client;

        public CommandHandler(QClient client)
        {
            this.client = client;
        }

        public void command(string cmd)
        {
            if (cmd.Length < 1) return;
            if (cmd.Substring(0, 1) != "/")
            {
                if (client.current != null)
                    client.current.sendMessage(cmd);
                else
                {
                    Program.WriteLine("！必须先打开一个会话才能发消息");
                }
                return;
            }
            string[] command = cmd.Split(' ');
            try
            {
                switch (command[0].Substring(1).ToLower())
                {
                    case "sel":
                        cmd_sel(command.Length < 2 ? "" : command[1]);
                        break;
                    case "fri":
                        cmd_fri(command.Length < 2 ? "" : command[1]);
                        break;
                    case "gro":
                        cmd_gro(command.Length < 2 ? "" : command[1]);
                        break;
                    case "con":
                        cmd_con(command.Length < 2 ? "" : command[1]);
                        break;
                    case "cof":
                        cmd_cof(long.Parse(command[1]));
                        break;
                    case "cog":
                        cmd_cog(long.Parse(command[1]));
                        break;
                    case "cot":
                        cmd_cot(long.Parse(command[1]), long.Parse(command[1]));
                        break;
                    default:
                        Program.WriteLine("！无法解析该指令");
                        break;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                Program.WriteLine("！指令需要更多参数");

            }
            catch (Exception e)
            {
                Program.WriteLine("！" + e.Message);
            }
        }

        public void cmd_sel(string id)
        {
            if (id.Length > 0)
            {
                client.current = client.conversations[int.Parse(id)];
                Console.Title = Program.TITLE + client.current.peername + " #" + client.current.peerid;
                client.refreshCurrent();
            }
            else
            {
                Console.Title = Program.TITLE + "未加入会话";
                Program.Clear();
                Console.BufferWidth = Console.WindowWidth;
                Console.BufferHeight = Console.WindowHeight;
                Program.WriteLine("\n\n===============\n已退出会话\n/con 列出会话列表\n" +
                "/fri 列出好友列表 \n/gro 列出群列表 \n" +
                "/sel [ID] 切换到指定会话\n/cof [QQ] 与指定好友聊天\n" +
                "/cog [GROUP] 与指定群聊天\n/cot [GROUP] [QQ] 与指定群中的群员临时聊天");
                client.current = null;
            }
        }

        public void cmd_con(string keyword = "")
        {
            Program.WriteLine("\n【会话列表】\n#ID\t消息\t名称\tPeerID");

            int i = 0;
            foreach (Conversation fi in client.conversations)
            {
                if (keyword.Length < 0 || fi.peerid.ToString().IndexOf(keyword) > -1 || fi.peername.IndexOf(keyword) > -1)
                {
                    Program.WriteLine("#" + i + "\t" + fi.messages.Count + "\t" + fi.peername + "\t" + fi.peerid);

                }
                i++;
            }
        }

        public void cmd_gro(string keyword = "")
        {
            Program.WriteLine("\n【群列表】\n#ID\t名称");

            foreach (IGroupInfo fi in client.groups)
            {
                if (keyword.Length < 0 || fi.Id.ToString().IndexOf(keyword) > -1 || fi.Name.IndexOf(keyword) > -1)
                {
                    Program.WriteLine("#" + fi.Id + "\t" + fi.Name);

                }
            }
        }

        public void cmd_fri(string keyword = "")
        {
            Program.WriteLine("\n【好友列表】\n#ID\t昵称");

            foreach (IFriendInfo fi in client.friends)
            {
                if (keyword.Length < 0 || fi.Id.ToString().IndexOf(keyword) > -1 || fi.Name.IndexOf(keyword) > -1)
                {
                    Program.WriteLine("#" + fi.Id + "\t" + fi.Name);

                }
            }
        }

        public void cmd_cof(long id)
        {
            if (client.getQName(id) == null)
                Program.WriteLine("！该账号不是您的好友");
            else
            {
                Conversation c = client.startConversation(id, client.getQName(id), 0);
                client.current = c;
                client.refreshCurrent();
            }
        }

        public void cmd_cog(long id)
        {
            if (client.getGName(id) == null)
                Program.WriteLine("！您不能访问该群");
            else
            {
                Conversation c = client.startConversation(id, client.getGName(id), 1);
                client.current = c;
                client.refreshCurrent();
            }
        }

        public void cmd_cot(long group, long id)
        {
            if (client.getGName(group) == null)
                Program.WriteLine("！您不能访问该群");
            else if (client.getGMemberName(group, id) == null)
                Program.WriteLine("！没有该群成员");
            else
            {
                Conversation c = client.startConversation(id, client.getGName(id), 2, group);
                client.current = c;
                client.refreshCurrent();
            }
        }
    }
}
