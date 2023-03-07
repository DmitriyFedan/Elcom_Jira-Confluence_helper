using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElcrumPokerBotDiscord
{
    public class ParticipantsCollection : IEnumerable<Participant> 
    {
        public List<Participant> ParticipantsList { get; set; }

        public ParticipantsCollection()
        {
            ParticipantsList = new List<Participant>();
        }


        public bool IsContain(string userName)
        {
            bool result = false;
            foreach (var item in ParticipantsList)
            {
                 if (item.UserName.Equals(userName))
                 {
                    result = true;
                 }
            }
            return result;
        }

        public void Add(Participant participant)
        {
            ParticipantsList.Add(participant);
        }


        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator<Participant> IEnumerable<Participant>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

       
    }
}
