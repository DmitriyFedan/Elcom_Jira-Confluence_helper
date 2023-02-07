using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ElcrumPokerBotDiscord
{
    public class DiscordBotMessageHandler
    {
        Dictionary<string, int> Estimates = new Dictionary<string, int>();

        List<string> Administrators = new List<string>()
        {
            "Dmitriy_Fedan",
            "Uglickih Viacheslav",
            "Chjen Nikita"
        };

        List<string> AvailableClients = new List<string>()
        {
                
        };

        public Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }


        //TODO  вынести логику отппавки сообщений в отдельный метод, а лучше класс 
        public Task MesagesHandler(SocketMessage msg)
        {
            if (msg.Author.IsBot)
            {
                return Task.CompletedTask;
            }
            //if (!Administrators.Contains(msg.Author.Username.ToString()) ||
            //    !AvailableClients.Contains(msg.Author.Username.ToString()))
            //{
            //    return Task.CompletedTask;
            //}

            switch (msg.Content)
            {
                case "/hello":

                    string username = msg.Author.Username;
                    if (AvailableClients.Contains(username))
                    {
                        msg.Channel.SendMessageAsync($" {msg.Author.Username}, вы уже участвуете в текущем Scrum Poker голосовании");
                        
                        break;
                    }
                    AvailableClients.Add(username);
                    msg.Channel.SendMessageAsync($" {msg.Author.Username}, добро пожаловать в новое Scrum Poker голосование");

                    break;

                case "/new":
                    Estimates.Clear();
                    msg.Channel.SendMessageAsync($" ===== Новое голосование ===== ");

                    break;

                // TODO сделать рассылку всем участникам 
                case "/result":
                    double result = 0;
                    string ResultMessage = "";
                    foreach (KeyValuePair<string, int> item in Estimates)
                    {
                        result += item.Value;
                        ResultMessage += $"{item.Key} - {item.Value.ToString()} \n";                       
                    }
                    if (Estimates.Count == 0)
                    {
                        msg.Channel.SendMessageAsync("Нет оценок");

                        break;
                    }
                    //msg.Channel.SendMessageAsync((result / Estimates.Count).ToString());
                    result = result / Estimates.Count;

                    msg.Channel.SendMessageAsync($"Средняя оценка - {result} \n"+ ResultMessage);

                    break;

                case "/clearClients":
                    if (Administrators.Contains(msg.Author.Username))
                    {
                        AvailableClients.Clear();
                        Estimates.Clear();
                        msg.Channel.SendMessageAsync($"{msg.Author.Username}, очистил список разрешенных пользователей и оценок");

                        break;
                    }

                    msg.Channel.SendMessageAsync($"У {msg.Author.Username}, нет прав на очистку разрешенных пользователей");

                    break;

                default:
                    if (int.TryParse(msg.Content, out int vote))
                    {
                        AddOrCorrectEstimate(msg.Author.Username, vote);

                        break;
                    }

                    msg.Channel.SendMessageAsync("Не удалось распознать");

                    break;

            }

            return Task.CompletedTask;
        }


        private void AddOrCorrectEstimate(string author, int estimate )
        {
            if (Estimates.ContainsKey(author))
            {
                Estimates[author] = estimate;
                return;
            }
            else
            {
                Estimates.Add(author, estimate);
                return;
            }
        }

      

    }
}
