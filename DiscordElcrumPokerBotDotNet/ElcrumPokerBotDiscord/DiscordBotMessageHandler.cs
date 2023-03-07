using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

namespace ElcrumPokerBotDiscord
{
    public class DiscordBotMessageHandler
    {
        // fields 
        private DiscordSocketClient _discordClient;

        // properties
        private Dictionary<string, int> Estimates { get; }

        List<string> Administrators { get; }

        List<string> AvailableClients { get; }


        //private ParticipantsCollection Participants = new ParticipantsCollection();

        List<SocketUser> DiscordParticipants { get; }

        public DiscordBotMessageHandler(DiscordSocketClient discordClient)
        {
            _discordClient = discordClient;

            Estimates = new Dictionary<string, int>();
            DiscordParticipants = new List<SocketUser> { };
            Administrators = new List<string>()
            {
                "Dmitriy_Fedan",
                "Uglickih Viacheslav",
                "Chjen Nikita"
            };

            

            discordClient.MessageReceived += MesagesHandler;
            discordClient.Log += Log;
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
            if (msg.Author.IsBot)
            {
                return Task.CompletedTask;
            }


            string userName = msg.Author.Username.ToString();
            ISocketMessageChannel messageChanel = msg.Channel;
            SocketUser author = msg.Author;


            switch (msg.Content)
            {

                case "/hello": //  подтверждаем участие или добавляем нового участника (вынести в отдельный метод)
                    CheckOrAddParticipant(msg.Author);

                    break;

                case "/initdb":
                    InitializeParticipantsFromDB();              

                    break;

                case "/new":
                    Estimates.Clear();
                    SendMessageToAllParticipants($" ===== Новое голосование ===== ");

                    break;

                case "/result":
                    SendResultToAllParticipants();

                    break;

                case "/clearAll": // todo  добавитьпроверку на  наличие прав на данное действие               
                    SendMessageToAllParticipants(
                        $"{msg.Author.Username}, очистил список разрешенных пользователей и оценок");
                    DiscordParticipants.Clear();
                    Estimates.Clear();

                    break;

                default:
                    if (TryHandledEstimate(msg.Content, userName))
                    {
                        if (DiscordParticipants.Count == Estimates.Count)
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

        private void AddOrCorrectEstimate(string author, int estimate)
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

        private void CheckOrAddParticipant(SocketUser socketUser)
        {
            if (DiscordPartisipantsContains(socketUser))
            {
                socketUser.SendMessageAsync(
                    $" {socketUser.Username}, вы уже участвуете в текущем Scrum Poker голосовании");
                return;
            }
            else
            {
                DiscordParticipants.Add(socketUser);

                WriteParticipantToDB(socketUser);

                socketUser.SendMessageAsync(
                    ($" {socketUser.Username}, добро пожаловать в новое Scrum Poker голосование"));
            }
        }

        private bool DiscordPartisipantsContains(SocketUser user)
        {
            bool contains = false;

            foreach (var participant in DiscordParticipants)
            {
                if (user.Id == participant.Id)
                {
                    contains = true;
                    break;
                }
            }

            return contains;
        }

        private void SendMessageToAllParticipants(string message)
        {
            foreach (var participant in DiscordParticipants)
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

        private bool
            TryHandledEstimate(string msgText,
                string userName) // добавить провероку на наличие данного юзера в голосовании 
        {
            if (int.TryParse(msgText, out int vote))
            {
                AddOrCorrectEstimate(userName, vote);
                return true;

            }

            return false;
        }


        public async Task InitializeParticipantsFromDB()
        {
            DiscordParticipants.Clear();

            using (AppContext db = new AppContext())
            {
                var dbParticipants = db.Users.ToList();
                foreach (var participant in dbParticipants)
                {
                   
                    //var socketClient = _discordClient.GetUser(participant.Id);
                    var socketClient =  await GetUserByIdAsync(participant.Id); //_discordClient.GetUser(participant.Id);
                    //var socketChanel = _discordClient.GetChannel(participant.Id);       
                    if (socketClient != null)
                    {
                        DiscordParticipants.Add(socketClient);
                    }
                }
            }
        }

        public async Task<SocketUser> GetUserByIdAsync(ulong userId)
        {
            var user = await _discordClient.GetUserAsync(userId);

            return user as SocketUser;
        }

        private void WriteParticipantToDB(SocketUser socketUser) // не сообщает что пользователь уже есть в  БД можно добавить это
        {
            Participant participant = new Participant(socketUser);

            using (AppContext db = new AppContext())
            {
                var dbList = db.Users.ToList();
                foreach (var user in dbList)
                {
                    if (user.Id == socketUser.Id)
                    {
                        return;
                    }
                }
                
                db.Users.Add(participant);
                db.SaveChanges();
            }
        }
    }
}
