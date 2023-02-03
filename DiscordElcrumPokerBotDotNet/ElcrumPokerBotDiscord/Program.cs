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

    internal class Program
    {

        Dictionary<string, int> Votes = new Dictionary<string, int>();

        List<string> AvailableClients = new List<string>()
            {
                "Vladimir Shtarev",
                "Dmitriy_Fedan",
                "Chjen Nikita"

            };
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
                                password: "5u738mbcyopE4")
                    }),
        
            };

            DiscordSocketClient client = new DiscordSocketClient(socketConfig);

            

            client.MessageReceived += CommandsHandler;
            client.Log += Log;

            string token = "";

            string path = Directory.GetParent(Directory.GetCurrentDirectory()).ToString();

            path = String.Concat(path, "\\Administration\\DiscordBotToken.txt");


            using (StreamReader reader = new StreamReader("C:\\Users\\FedanDA\\Desktop\\FEDANDA\\My repos\\Scrum Helpers\\Administration\\DiscordBotToken.txt"))
            {
                token = await reader.ReadToEndAsync();

            }

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            Console.ReadLine();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task CommandsHandler(SocketMessage msg)
        {
            if (msg.Author.IsBot || !AvailableClients.Contains(msg.Author.Username.ToString()))
            {
                return Task.CompletedTask;
            }
            switch (msg.Content)
            {
                case "/hallo":
                    Console.WriteLine(msg.Content);
                    msg.Channel.SendMessageAsync($"Привет, {msg.Author.Username}");
                    break;
                case "/new":
                    Votes.Clear();
                    msg.Channel.SendMessageAsync($" ===== Новое голосование ===== ");

                    break;

                case "/result":
                    double result = 0;
                    foreach (KeyValuePair<string, int> item  in Votes)
                    {
                        result += item.Value;
                    }
                    if (Votes.Count == 0)
                    {
                        msg.Channel.SendMessageAsync("Нет оценок");
                        break;
                    }
                    msg.Channel.SendMessageAsync((result / Votes.Count).ToString());
                    break;

                default:
                    if (int.TryParse(msg.Content, out int vote))
                    {
                        Votes.Add(msg.Author.Username, vote);
                        break;
                    }
                    

                    else
                    {
                        msg.Channel.SendMessageAsync("Не удалось распознать");
                        break;
                    }


            }

            return Task.CompletedTask;
                
        }

    }

    
}