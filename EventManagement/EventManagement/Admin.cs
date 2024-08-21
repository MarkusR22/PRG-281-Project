using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement
{
    
    public class Admin
    {
        public enum AdminMenuOptions
        {
            View_Upcoming_Events = 1,
            Create_New_Event,
            Register_New_Organizer,
            Past_Feedback,
            Edit_Details,
            Log_out
        }

        public void DisplayMenu()
        {

                Console.WriteLine("Admin Menu:");

                foreach (AdminMenuOptions option in Enum.GetValues(typeof(AdminMenuOptions)))
                {
                    string optionName = option.ToString().Replace("_", " ");
                    Console.WriteLine($"{(int)option}. {optionName}");
                }

                Console.Write("Select an option (1-6): ");
                string input = Console.ReadLine().Trim();
                

                if (Enum.TryParse(input, out AdminMenuOptions chosenOption) && Enum.IsDefined(typeof(AdminMenuOptions), chosenOption))
                {
                    switch (chosenOption)
                    {
                        case AdminMenuOptions.View_Upcoming_Events:

                            break;
                        case AdminMenuOptions.Create_New_Event:


                            break;
                        case AdminMenuOptions.Register_New_Organizer:

                            break;
                        case AdminMenuOptions.Past_Feedback:

                            break;
                        case AdminMenuOptions.Edit_Details:

                            break;
                        case AdminMenuOptions.Log_out:

                            break;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid option, please try again.");
                }
            
        }

        public static void BackToMainMenu()
        {
            Console.WriteLine("Press any key to return to the admin menu...");
            Console.ReadKey();
            Console.Clear();
        }

    }
}
