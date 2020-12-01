using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;

namespace QChatClient
{
    class EventHandler : IGroupMessage, IFriendMessage, ITempMessage
    {
        QClient qc;

        public EventHandler(QClient client)
        {
            qc = client;
        }

        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e)
        {
            qc.chainReceived(e.Sender, e.Chain, 0);
            return false;
        }

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e)
        {
            qc.chainReceived(e.Sender.Group, e.Sender, e.Chain);
            return false;
        }

        public async Task<bool> TempMessage(MiraiHttpSession session, ITempMessageEventArgs e)
        {
            qc.chainReceived(e.Sender, e.Chain, 2);
            return false;
        }
    }
}
