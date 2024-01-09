using Discord.WebSocket;

namespace ElcrumPokerBotDiscord.Models
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
