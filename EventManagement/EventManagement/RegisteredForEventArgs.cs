using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement
{
    public class RegisteredForEventArgs : EventArgs
    {
        public string entryCode { get; set; }
    }
}
