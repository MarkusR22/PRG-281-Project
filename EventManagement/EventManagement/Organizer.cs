using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement
{
    internal class Organizer : User
    {
        public Organizer(int id, string userName, string password) : base(id, userName, password)
        {

        }

        public enum OrganizerMenuOptions
        {
            View_Upcoming_Events = 1,
            Create_New_Event,
            Register_New_Organizer,
            Past_Feedback,
            Edit_Details,
            Log_out
        }

        public void SetUserName(string userName)
        {
            base.userName = userName;
        }

        public void SetPassword(string password)
        {
            base.password = password;
        }

        public override void DisplayMenu()
        {
            while (true)
            {
                Console.WriteLine("Organizer Menu:");

                foreach (OrganizerMenuOptions option in Enum.GetValues(typeof(OrganizerMenuOptions)))
                {
                    string optionName = option.ToString().Replace("_", " ");
                    Console.WriteLine($"{(int)option}. {optionName}");
                }

                Console.Write("Select an option (1-6): ");
                string input = Console.ReadLine().Trim();


                if (Enum.TryParse(input, out OrganizerMenuOptions chosenOption) && Enum.IsDefined(typeof(OrganizerMenuOptions), chosenOption))
                {
                    switch (chosenOption)
                    {
                        case OrganizerMenuOptions.View_Upcoming_Events:

                            break;
                        case OrganizerMenuOptions.Create_New_Event:

                            break;
                        case OrganizerMenuOptions.Register_New_Organizer:

                            break;
                        case OrganizerMenuOptions.Past_Feedback:

                            break;
                        case OrganizerMenuOptions.Edit_Details:

                            break;
                        case OrganizerMenuOptions.Log_out:

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

        public override void Logout()
        {
            Console.WriteLine("Admin logout successful!");
        }

        public static void BackToMainMenu()
        {
            Console.WriteLine("Press any key to return to the organizer menu...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
