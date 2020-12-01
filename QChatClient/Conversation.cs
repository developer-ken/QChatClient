using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mirai_CSharp;
using Mirai_CSharp.Models;

namespace QChatClient
{
    class Conversation : IEquatable<Conversation>
    {
        public MiraiHttpSession session;
        /// <summary>
        /// 好友=0 | 群聊=1 | 临时会话=2
        /// </summary>
        public short type;
        public long tgroup;
        public long peerid;
        public string peername;

        public List<KeyValuePair<IBaseInfo, IMessageBase[]>> messages = new List<KeyValuePair<IBaseInfo, IMessageBase[]>>();

        public bool Equals(Conversation other)
        {
            return (other != null) && this.peerid.Equals(other.peerid) && this.type.Equals(other.type);
        }

        public void sendMessage(string msg)
        {
            switch (type)
            {
                case 0:
                    session.SendFriendMessageAsync(peerid, new IMessageBase[] { new PlainMessage(msg) }).Wait();
                    break;
                case 1:
                    session.SendGroupMessageAsync(peerid, new IMessageBase[] { new PlainMessage(msg) }).Wait();
                    break;
                case 2:
                    session.SendTempMessageAsync(peerid,tgroup, new IMessageBase[] { new PlainMessage(msg) }).Wait();
                    break;
            }
        }
    }
}
