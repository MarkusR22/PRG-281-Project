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

       //Notifying the organizer that his event was approved
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

        //Notifying the organizer and attending participants that the event was cancelled
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

        //Notifying participant that they succesfully registered for event
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
            Console.WriteLine("\nSuccessfully registered for event!");
            Console.WriteLine($"Your entry code is: {e.entryCode}");
            Console.ResetColor();
        }

        //Notifying participant that they succesfully deregistered for event
        public void OnDeregisteredForEvent(object sender, EventArgs e)
        {
            Console.ForegroundColor= ConsoleColor.Red;
            Console.Write("Cancelling registration for event");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.WriteLine("\nYou have successfully cancelled your registration for the event.");
            Console.ResetColor();
        }

        
    }
}
