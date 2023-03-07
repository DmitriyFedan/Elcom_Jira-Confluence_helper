using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElcrumPokerBotDiscord
{
    public class Participant
    {
        public ulong Id { get; set; }
        public string UserName { get; set; }
        
        public Participant(SocketUser socketUser)
        {
            Id = socketUser.Id;
            UserName = socketUser.Username;
        }

        public Participant()
        {
            Id = 0;
            UserName = "Empty Name";
        }
    }
}
