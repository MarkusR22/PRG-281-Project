using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement
{
    public abstract class User
    {
        protected int id { get; set; }
        protected string userName { get; set; }
        protected string password { get; set; }

        


        

        public abstract void DisplayMenu();
        //public abstract void Login();
        //public abstract void SignUp();
        public abstract void Logout();

        public User(int id, string userName, string password)
        {
            this.id = id;
            this.userName = userName;
            this.password = password;
        }
        

    }
}
