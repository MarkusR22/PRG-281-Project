using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement
{
    public interface IAdmin
    {
        void ViewUpcomingEvents();
        void CreateNewEvents();
        void RegisterOrganizer();
        void PastFeedback();
        void EditAdminDetails();
        void EditEventDetails();
        void ViewParticipants();
        void RemoveParticipant();

    }
}
