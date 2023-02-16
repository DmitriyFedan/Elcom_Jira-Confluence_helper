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


        private Dictionary<string, int> Estimates { get; }

        List<string> Administrators { get; }

        List<string> AvailableClients { get; }
       

        //private ParticipantsCollection Participants = new ParticipantsCollection();

        List<SocketUser> Participants { get; }

        public DiscordBotMessageHandler()
        {
            Estimates = new Dictionary<string, int>();

            Participants = new List<SocketUser> { };

            Administrators = new List<string>()
            {
                "Dmitriy_Fedan",
                "Uglickih Viacheslav",
                "Chjen Nikita"
            };
        }

        public Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }


        //TODO  вынести логику отппавки сообщений в отдельный метод, а лучше класс  

        //todo  класс рассыльщик, должен отправлять сообщения как одному пользователю так и делать массовую рассылку,
        //возможно читать  участников из БД, 
        public Task MesagesHandler(SocketMessage msg)
        {
            if (msg.Author.IsBot )
            {
                return Task.CompletedTask;
            }
            //if (!Administrators.Contains(msg.Author.Username.ToString()) ||
            //    !AvailableClients.Contains(msg.Author.Username.ToString()))
            //{
            //    return Task.CompletedTask;
            //}
            string userName = msg.Author.Username.ToString();
            ISocketMessageChannel messageChanel = msg.Channel;
            SocketUser author = msg.Author;
            switch (msg.Content)
            {
                
                case "/hello": //  подтверждаем участие или добавляем нового участника (вынести в отдельный метод)
                    CheckOrAddParticipant(msg.Author);

                    break;

                case "/new":
                    Estimates.Clear();
                    SendMessageToAllParticipants($" ===== Новое голосование ===== ");
                                        
                    break;
           
                case "/result":
                    SendResultToAllParticipants();

                    break;

                case "/clearAll": // todo  добавитьпроверку на  наличие прав на данное действие               
                    SendMessageToAllParticipants($"{msg.Author.Username}, очистил список разрешенных пользователей и оценок");
                    Participants.Clear();
                    Estimates.Clear();

                    break;

                default:
                    if (TryHandledEstimate(msg.Content, userName))
                    {
                        if (Participants.Count == Estimates.Count)
                        {
                            SendResultToAllParticipants();
                        }
                    }
                    else 
                    {
                        msg.Author.SendMessageAsync("не удалось распознать");
                    }
                            
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

        private void CheckOrAddParticipant(SocketUser user)
        {
            string userName = user.Username.ToString();
            
            if (PartisipantsContains(user.Username))
            {
                user.SendMessageAsync($" {userName}, вы уже участвуете в текущем Scrum Poker голосовании");
                return;
            }
            else
            {
                Participants.Add(user);
                user.SendMessageAsync(($" {userName}, добро пожаловать в новое Scrum Poker голосование"));
            }
        }

        private bool PartisipantsContains(string userName)
        {
            bool contains = false;

            foreach (var participant in Participants)
            {
                if (participant.Username.Equals(userName))
                {
                    contains = true;
                }
            }

            return contains;
        }

        private void SendMessageToAllParticipants(string message)
        {
            foreach (var participant in Participants)
            { 
                participant.SendMessageAsync(message);
            }
        }

        private void SendResultToAllParticipants()
        {
            double result = 0;
            string ResultMessage = "";
            foreach (KeyValuePair<string, int> item in Estimates)
            {
                result += item.Value;
                ResultMessage += $"{item.Key} - {item.Value} \n";
            }

            if (Estimates.Count == 0)
            {
                SendMessageToAllParticipants("Нет оценок");
            }

            result /= Estimates.Count;
            SendMessageToAllParticipants($"Средняя оценка - {result} \n" + ResultMessage);
        }

        private bool TryHandledEstimate( string msgText, string userName) // добавить провероку на наличие данного юзера в голосовании 
        {
            if (int.TryParse(msgText, out int vote))
            {
                AddOrCorrectEstimate(userName, vote);
                return true;

            }

            else
            {
                return false;
            }

        }

    }
}
