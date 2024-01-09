using Discord.WebSocket;
using ElcrumPokerBotDiscord;
using ElcrumPokerBotDiscord.Models;
using MauiDiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiDiscordBot.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        MessageHandlerService _messageHandlerService;

        public ObservableCollection<Participant> Participants { get; }

        private string _name;

        public ICommand AddUser { get; set; }
       

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainPageViewModel() {

            var discordClientSeervice = new DiscordClientService();
            _messageHandlerService = new MessageHandlerService(discordClientSeervice);
            Participants = new ObservableCollection<Participant>();

            _messageHandlerService.UpdateParticipantsEvent += OnUpdateParticipantsList;
            Name = "Elcrum Poker Bot";
            AddUser = new Command(UpdateParticipantsAction);
        }


        public void OnPropertyChanged([CallerMemberName] string propertyName = "") 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateParticipantsAction()
        {
            OnUpdateParticipantsList(this, EventArgs.Empty);
        }
        private void OnUpdateParticipantsList(object sender, EventArgs e)
        {
            
            Application.Current.Dispatcher.DispatchAsync((Action)(() =>
            {
                Participants.Clear();
                foreach (var socketUser in _messageHandlerService.DiscordParticipants)
                {
                    Participants.Add(new Participant(socketUser));
                }
            }));
        }
            
        

    }
}
