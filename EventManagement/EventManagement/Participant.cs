using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement
{
    internal class Participant
    {
        public enum ParticipantMenuOptions
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
            while (true)
            {
                Console.WriteLine("Participant Menu:");

                foreach (ParticipantMenuOptions option in Enum.GetValues(typeof(ParticipantMenuOptions)))
                {
                    string optionName = option.ToString().Replace("_", " ");
                    Console.WriteLine($"{(int)option}. {optionName}");
                }

                Console.Write("Select an option (1-6): ");
                string input = Console.ReadLine().Trim();
                IMenu selectedOption = null;

                if (Enum.TryParse(input, out ParticipantMenuOptions chosenOption) && Enum.IsDefined(typeof(ParticipantMenuOptions), chosenOption))
                {
                    switch (chosenOption)
                    {
                        case ParticipantMenuOptions.View_Upcoming_Events:

                            break;
                        case ParticipantMenuOptions.Create_New_Event:

                            break;
                        case ParticipantMenuOptions.Register_New_Organizer:

                            break;
                        case ParticipantMenuOptions.Past_Feedback:

                            break;
                        case ParticipantMenuOptions.Edit_Details:

                            break;
                        case ParticipantMenuOptions.Log_out:

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
    }
}
