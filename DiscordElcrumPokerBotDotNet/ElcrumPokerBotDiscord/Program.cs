using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Rest;
using Discord.Net.Udp;
using Discord.Net.WebSockets;
using Newtonsoft.Json.Linq;


namespace ElcrumPokerBotDiscord
{

    public class Program
    {
        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            DiscordSocketConfig config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.All
            };

            DiscordSocketClient discordClient = new DiscordSocketClient(config);   //  or with  config =>   socketConfig

            string token = GetTokenFromFile();
            await discordClient.LoginAsync(TokenType.Bot, token);
            await discordClient.StartAsync();

            DiscordBotMessageHandler messageHandler = new DiscordBotMessageHandler(discordClient);
            await messageHandler.InitializeParticipantsFromDB();

            Console.ReadLine();
        }

        private string GetTokenFromFile()
        {
            string curentFolder = Environment.CurrentDirectory;
            string debugFolder = "\\DiscordElcrumPokerBotDotNet\\ElcrumPokerBotDiscord\\bin\\Debug\\net6.0";
            string administrationFolder = "\\Administration\\DiscordBotToken.txt";
            string tokenPath = curentFolder.Replace(debugFolder, administrationFolder);

            using (StreamReader reader = new StreamReader(tokenPath))
            {
                string token = reader.ReadToEnd();
                return token;
            }
        }
    }

    
}