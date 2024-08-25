using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement
{
    internal class Participant : User
    {
        EventManager eventManager = new EventManager();
        //private static string connectionString = "Data Source=MACHINE;Initial Catalog=EventManagementTemp;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"; // Caydan
        //private static string connectionString = "Data Source=TIMOTHY\\MSSQLSERVER09;Initial Catalog=EventManagementTemp;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"; //Joseph's DB connection
        //private static string connectionString = "Data Source=DESKTOP-TDBJOM7;Initial Catalog=EventManagement;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"; //Markus' connection string
        //private static string connectionString = "Data Source=EE-GAMINGPC;Initial Catalog=EventManagementTheuns;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"; //Theuns db string

        public Participant(int id, string userName, string password) : base(id, userName, password)
        {

        }

        public void SetUserName(string userName)
        {
            base.userName = userName;
        }

        public void SetPassword(string password)
        {
            base.password = password;
        }
        public enum ParticipantMenuOptions
        {
            Search_Display = 1,
            View_Events,
            Logout,
            User_details,
            
        }
        public enum UserDetails
        {
            View =1,
            Edit
        }

        public override void Logout()
        {
            Console.WriteLine("Admin logout successful!");
        }



        public override void DisplayMenu()
        {
            while (true)
            {
                Console.WriteLine("Participant Menu:");

                foreach (ParticipantMenuOptions option in Enum.GetValues(typeof(ParticipantMenuOptions)))
                {
                    string optionName = option.ToString().Replace("_", " ");
                    Console.WriteLine($"{(int)option}. {optionName}");
                }

                Console.Write("Select an option (1-4): ");
                string input = Console.ReadLine().Trim();
                

                if (Enum.TryParse(input, out ParticipantMenuOptions chosenOption) && Enum.IsDefined(typeof(ParticipantMenuOptions), chosenOption))
                {
                    switch (chosenOption)
                    {
                        case ParticipantMenuOptions.Search_Display:
                            SearchDisplay(eventManager.GetEvents());
                            break;
                        case ParticipantMenuOptions.View_Events:

                            break;
                        case ParticipantMenuOptions.Logout:

                            break;
                        case ParticipantMenuOptions.User_details:
                            foreach (UserDetails option in Enum.GetValues(typeof(UserDetails)))
                            {
                                string optionName = option.ToString().Replace("_", " ");
                                Console.WriteLine($"{(int)option}. {optionName}");
                            }
                            break;

                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid option, please try again.");
                    continue;
                }

            }
        }


        public static void BackToMainMenu()
        {
            Console.WriteLine("Press any key to return to the participant menu...");
            Console.ReadKey();
            Console.Clear();
        }

        public void SearchDisplay(List<Event> events)
        {
            try
            {
                Console.WriteLine("Enter ID of event you want to search for:");
                int id = int.Parse(Console.ReadLine());
                bool found = false;
                foreach (var item in events)
                {
                    if (item.EventId == id)
                    {
                        eventManager.DisplayEventDetails(item);
                        found = true;
                    }
                }

                if (!found)
                {
                    throw new Exception();
                }
            }
            catch
            {
                Console.WriteLine("ID does not exist");
                Console.WriteLine("Retry? [Y/N]");
                string retry = Console.ReadLine();

                if (retry.ToLower() == "y")
                {
                    SearchDisplay(events);
                }
                else if (retry.ToLower() == "n")
                {
                    DisplayMenu();
                }
            }
        }

    }
}
