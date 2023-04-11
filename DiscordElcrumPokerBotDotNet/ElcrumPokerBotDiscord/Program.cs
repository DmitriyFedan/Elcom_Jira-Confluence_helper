using Discord;
using Discord.WebSocket;
using Discord.Net.Rest;
using Discord.Net.WebSockets;

using System.Net;

namespace ElcrumPokerBotDiscord
{
    public class Program
    {
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