using Discord;
using Discord.WebSocket;
using Discord.Net.Rest;
using Discord.Net.WebSockets;


using System.Net;

using Newtonsoft.Json.Linq;
using System.Reflection;
using Discord.Commands;

namespace ElcrumPokerBotDiscord
{
    public class Program
    {
        public DiscordSocketClient _discordClient;
        public CommandService _commandService;
        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            var proxyAdres = new Uri("http://192.168.36.253:5454");
            WebProxy elcomProxy = new WebProxy(proxyAdres, false, null);
            DiscordSocketConfig config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.All,
                RestClientProvider = DefaultRestClientProvider.Create(true),
                WebSocketProvider = DefaultWebSocketProvider.Create(elcomProxy)
            };

            string token = GetTokenFromFile();

            _discordClient = new DiscordSocketClient(config);   //  or with  config =>   socketConfig
            _commandService = new CommandService();

            
            CommandHandler commandHandler = new CommandHandler(_discordClient, _commandService);
            await commandHandler.InstallCommands();
            
            // DiscordBotMessageHandler messageHandler = new DiscordBotMessageHandler(discordClient);
            //await messageHandler.InitializeParticipantsFromDB();

            await _discordClient.LoginAsync(TokenType.Bot, token);
            await _discordClient.StartAsync();

            await Task.Delay(-1);
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