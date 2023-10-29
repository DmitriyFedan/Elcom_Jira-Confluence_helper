using Azure.Identity;
using Discord;
using Discord.WebSocket;
using ElcrumPokerBotDiscord.Models;
using Microsoft.IdentityModel.Tokens;
using System.Net.WebSockets;

namespace ElcrumPokerBotDiscord
{
    public class DiscordBotMessageHandler
    {
        // fields 
        private DiscordSocketClient _discordClient;

        // properties
        private Dictionary<string, int> Estimates { get; }

        List<SocketUser> DiscordParticipants { get; }

        public DiscordBotMessageHandler(DiscordSocketClient discordClient)
        {
            _discordClient = discordClient;

            Estimates = new Dictionary<string, int>();
            DiscordParticipants = new List<SocketUser> { };

            discordClient.MessageReceived += MesagesHandler;
            discordClient.Log += Log;
            discordClient.ButtonExecuted += ButtonHandler;
        }

        public Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// /add_123456789123456      invite  user  by id 
        ///  /initdb
        ///  
        /// 
        ///  /hello
        ///  /clearAll
        ///  /result
        ///  /new
        ///  
        ///  
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>


        public Task MesagesHandler(SocketMessage msg)
        {
            if (msg.Author.IsBot)
            {
                return Task.CompletedTask;
            }

            if (msg.Content.Contains("/add_"))
            {
                string invitedUser = msg.Content.Replace("/add_", "");



                if (!string.IsNullOrEmpty(invitedUser) && ulong.TryParse(invitedUser, out ulong invitedUserId ))
                {
                    
                    var invitedDiscordUser = _discordClient.GetUser(invitedUserId);

                    if (invitedDiscordUser != null)
                    {
                        SendInviteTo(msg.Author, invitedDiscordUser);
                    }
                }

                //string userName = invitedUser.Split('#')[0];
                //string descriminator = invitedUser.Split('#')[1];


                //if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(descriminator))
                //{
                //    var invitedDiscordUser = _discordClient.GetUser(userName, descriminator);

                //    if (invitedDiscordUser != null)
                //    {
                //        SendInviteTo(msg.Author, invitedDiscordUser);
                //    }

                //}
                return Task.CompletedTask;
            }

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


                case "/test":

                    var builder = new ComponentBuilder();
                    builder.WithButton("Принять", "accept", ButtonStyle.Success);
                    builder.WithButton("Отклонить", "reject", ButtonStyle.Danger);

                    msg.Author.SendMessageAsync("mesage", components: builder.Build());

                    break;

                case "/newvote":

                //var vote = new VoteFectory();


                default:


                    //var user = _discordClient.GetUser(ulong.Parse(msg.Content));

                    if (TryHandledEstimate(msg.Content, msg.Author.Username.ToString()))
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

        private void SendInviteTo(SocketUser voteCreator, SocketUser invitedSocketUser)
        {
            var builder = new ComponentBuilder();
            builder.WithButton("Принять", "accept", ButtonStyle.Success, row: 0);
            builder.WithButton("Отклонить", "reject", ButtonStyle.Danger, row: 0);

            invitedSocketUser.SendMessageAsync($"{voteCreator.Username.ToString()} предлагает вам принять участие в голосовании", components: builder.Build());
            
            
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

        private bool TryHandledEstimate(string msgText, string userName) // добавить провероку на наличие данного юзера в голосовании 
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
                    if (socketClient != null && !!socketClient.IsBot)
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

        private async Task ButtonHandler(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case "accept":
                  
                    CheckOrAddParticipant(component.User);


                    break;

                case "reject":
                    //TODO implement this logic
                    break;
            }
        }
    }
}
