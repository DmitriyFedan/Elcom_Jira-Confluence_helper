using Discord.WebSocket;

namespace ElcrumPokerBotDiscord.Models
{
    public class Vote
    {
        public int Id { get; set; }

        public List<Participant> Participants { get; set; }

        public Dictionary<string, int> Estimates { get; }

        public List<SocketUser> DiscordParticipants { get; }

        public Vote()
        {

        }
    }
}
