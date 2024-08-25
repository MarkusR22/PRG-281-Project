using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace EventManagement
{
    public class NotifyService
    {
        public NotifyService()
        {
            Admin.EventApproved += OnEventApproved;
        }

        public void OnEventApproved(object sender, EventArgs e)
        {
            Console.Write("Notifying organizer");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.WriteLine("\nOrganizer has been notified that his event was approved.");
        }
        
    }
}
