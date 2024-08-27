using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement
{
    public interface IParticipant
    {
        //1
        void DisplayAllUpcoming();
        //1

        //2
        void RegisterForEvent();
        //2

        //3
        void ViewRegisteredEvents();
        //3

        //4
        void CancelRegistration();
        //4

        //5
        void SubmitFeedback();
        //5

        //Default
        void DisplayMenu();
        //Default
    }
}
