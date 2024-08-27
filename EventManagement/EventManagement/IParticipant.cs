using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement
{
    public interface IParticipant
    {
        List<(int eventId, string eventName)> SearchEvents(bool showExitMessage = true);
        void ViewEventDetails();
        void RegisterForEvent();
        void ViewRegisteredEvents();
        void CancelRegistration();
        void SubmitFeedback();
        void DisplayMenu();
    }
}
