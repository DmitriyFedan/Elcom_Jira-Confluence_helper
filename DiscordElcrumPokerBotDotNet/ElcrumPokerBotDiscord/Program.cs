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


namespace ElcrumPokerBotDiscord
{

    public class Program
    {

        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();


        public static class ElcomWebSocketProvider
        {
            public static readonly WebSocketProvider Instance = DefaultWebSocketProvider.Create();
        }

        private async Task MainAsync()
        {

            DiscordSocketConfig socketConfig = new DiscordSocketConfig()
            {
                //https://stackoverflow.com/questions/29856543/httpclient-and-using-proxy-constantly-getting-407
                //RestClientProvider = DefaultRestClientProvider.Create(true), // или скорее всего придется сделать свой класс типа DefaultRestClient, только чтобы запросы шли через прокси
                RestClientProvider = ElcomRestClientProvider.Instance,
                UdpSocketProvider = DefaultUdpSocketProvider.Instance, //тут похоже нужно добавить для Udp прокси, пока хз как это делать
                WebSocketProvider = DefaultWebSocketProvider.Create(new WebProxy
                    {
                            Address = new Uri($"http://192.168.0.10:8080"),
                            BypassProxyOnLocal = false,
                            UseDefaultCredentials = false,

                            // *** These creds are given to the proxy server, not the web server ***
                            Credentials = new NetworkCredential(
                                userName: "fedanda",
                                password: "+++++++++++")
                    }),
        
            };

            DiscordSocketClient discordClient = new DiscordSocketClient();   //  or with  config =>   socketConfig

            DiscordBotMessageHandler messageHandler = new DiscordBotMessageHandler(discordClient);
            ////фыафыафыавфыва


            string token = "";

            //string path = Directory.GetParent(Directory.GetCurrentDirectory()).ToString();
            //path = String.Concat(path, "\\Administration\\DiscordBotToken.txt");
            string curentFolder = Environment.CurrentDirectory;
            string debugFolder = "\\DiscordElcrumPokerBotDotNet\\ElcrumPokerBotDiscord\\bin\\Debug\\net6.0";
            string administrationFolder = "\\Administration\\DiscordBotToken.txt";
            string tokenPath = curentFolder.Replace(debugFolder, administrationFolder);

            using (StreamReader reader = new StreamReader(tokenPath))
            {
                token = await reader.ReadToEndAsync();

            }

            await discordClient.LoginAsync(TokenType.Bot, token);
            await discordClient.StartAsync();

            Console.ReadLine();
        }



        
        

        private Task CommandHadler()
        {
            return Task.CompletedTask;
        }
    }

    
}