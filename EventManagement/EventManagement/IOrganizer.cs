using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement
{
    public interface IOrganizer
    {
        void DisplayOrganizedEvents();
        void DisplayMenu();
        void DisplayEventDetails(Event ev);
        void EditEvent(Event ev);
    }
}
