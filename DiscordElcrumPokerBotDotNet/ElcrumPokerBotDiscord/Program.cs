using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;


namespace ElcrumPokerBotDiscord
{

    internal class Program
    {

        Dictionary<string, int> Votes = new Dictionary<string, int>();

        List<string> AvailableClients = new List<string>()
            {
                "Vladimir Shtarev",
                "Dmitriy_Fedan"

            };
        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();




        private async Task MainAsync()
        {
            DiscordSocketClient client = new DiscordSocketClient();

            

            client.MessageReceived += CommandsHandler;
            client.Log += Log;

            string token = "";

            string path = Directory.GetParent(Directory.GetCurrentDirectory()).ToString();

            path = String.Concat(path, "\\Administration\\DiscordBotToken.txt");


            using (StreamReader reader = new StreamReader("G:\\WORKED\\Python\\Elcrum Helpers\\Administration\\DiscordBotToken.txt"))
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