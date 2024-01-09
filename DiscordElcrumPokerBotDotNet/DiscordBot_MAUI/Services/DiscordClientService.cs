using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MauiDiscordBot.Services.Interfaces;

namespace MauiDiscordBot.Services
{
    public class DiscordClientService : IDiscordClientService
    {
        public DiscordSocketClient DiscordClient { get; private set; }

        public DiscordClientService()
        {

            MainAsync().GetAwaiter();
        }

        private async Task MainAsync()
        {
            //var proxyAdres = new Uri("http://192.168.36.253:5454");
            //WebProxy elcomProxy = new WebProxy(proxyAdres, false, null);
            DiscordSocketConfig config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.All,
                //RestClientProvider = DefaultRestClientProvider.Create(true),
                //WebSocketProvider = DefaultWebSocketProvider.Create(elcomProxy)
            };
            string token = GetTokenFromFile();

            DiscordClient = new DiscordSocketClient(config);   //  or with  config =>   socketConfig
                                                               //_commandService = new CommandService();
                                                               //CommandHandler commandHandler = new CommandHandler(_discordClient, _commandService);
                                                               //await commandHandler.InstallCommands();

            //MessageHandlerService messageHandler = new MessageHandlerService(_discordClient);

            await DiscordClient.LoginAsync(TokenType.Bot, token);
            await DiscordClient.StartAsync();
            //await messageHandler.InitializeParticipantsFromDB();

            await Task.Delay(-1);
        }


        private string GetTokenFromFile()
        {
            return "qwetrq"
            //string curentFolder = Environment.CurrentDirectory;
            //string debugFolder = "\\DiscordElcrumPokerBotDotNet\\ElcrumPokerBotDiscord\\bin\\Debug\\net6.0";
            //string administrationFolder = "\\Administration\\DiscordBotToken.txt";
            //string tokenPath = curentFolder.Replace(debugFolder, administrationFolder);

            //using (StreamReader reader = new StreamReader(tokenPath))
            //{
            //    string token = reader.ReadToEnd();
            //    //return token;
            //    
            //}
        }
    }
}
