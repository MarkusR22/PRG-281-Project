using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace EventManagement
{
    public class NotifyService
    {
        public NotifyService()
        {
            Admin.EventApproved += OnEventApproved;
            Admin.EventCancelled += OnEventCancelled;
            Participant.RegisteredForEvent += OnRegisteredForEvent; 
        }

        public void OnEventApproved(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Notifying organizer");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.WriteLine("\nOrganizer has been notified that his event was approved.");
            Console.ResetColor();
        }

        public void OnEventCancelled(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Notifying organizer");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.WriteLine("\nOrganizer has been notified that his event was cancelled.");
            Console.Write("Notifying attendees");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.WriteLine("\nAll attendees have been notified that the event was cancelled.");
            Console.ResetColor();
        }

        public void OnRegisteredForEvent(object sender, RegisteredForEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Registering for event");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.WriteLine("\nsuccessfully registered for event!");
            Console.WriteLine($"Your entry code is: {e.entryCode}");
            Console.ResetColor();
        }

    }
}
