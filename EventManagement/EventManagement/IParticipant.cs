using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement
{
    public interface IParticipant
    {
        void SearchDisplay();
        void ViewEvents();
        void ViewParticipantDetails();
        void EditParticipantDetails();
        void RSVP();
        void ViewRSVPEvents();
        void ViewAgenda();
        void SupportForEvent();
        void CancelRSVP();
        void GiveFeedback();
    }
}
