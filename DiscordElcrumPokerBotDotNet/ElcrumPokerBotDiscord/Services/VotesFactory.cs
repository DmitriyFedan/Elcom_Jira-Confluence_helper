using Discord.WebSocket;
using ElcrumPokerBotDiscord.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElcrumPokerBotDiscord.Services
{
    public static class VotesFactory
    {

        public static Vote CreateVote()
        {

            var newVote = new Vote();

            return newVote;
        }

        private static Task MessageHandler(SocketMessage msg)
        {
            if (msg.Author.IsBot)
            {
                return Task.CompletedTask;
            }

            switch (msg.Content)

            return Task.CompletedTask;
        }

    }
}
