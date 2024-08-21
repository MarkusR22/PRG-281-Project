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
                            selectedOption = new ViewUpcomingEventsOption();
                            break;
                        case ParticipantMenuOptions.Create_New_Event:
                            selectedOption = new CreateNewEventOption();
                            break;
                        case ParticipantMenuOptions.Register_New_Organizer:
                            selectedOption = new RegisterNewOrganizerOption();
                            break;
                        case ParticipantMenuOptions.Past_Feedback:
                            selectedOption = new PastFeedbackOption();
                            break;
                        case ParticipantMenuOptions.Edit_Details:
                            selectedOption = new EditDetailsOption();
                            break;
                        case ParticipantMenuOptions.Log_out:
                            selectedOption = new LogoutOption();
                            break;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid option, please try again.");
                    continue;
                }

                selectedOption.Execute();
            }
        }

        public class ViewUpcomingEventsOption : IMenu
        {
            public void Execute()
            {
                Console.Clear();
                Console.WriteLine("Viewing upcoming events...");
                // Implement the logic to view upcoming events
                BackToMainMenu();
            }
        }

        public class CreateNewEventOption : IMenu
        {
            public void Execute()
            {
                Console.Clear();
                Console.WriteLine("Creating a new event...");
                // Implement the logic to create a new event
                BackToMainMenu();
            }
        }

        public class RegisterNewOrganizerOption : IMenu
        {
            public void Execute()
            {
                Console.Clear();
                Console.WriteLine("Registering a new organizer...");
                // Implement the logic to register a new organizer
                BackToMainMenu();
            }
        }

        public class PastFeedbackOption : IMenu
        {
            public void Execute()
            {
                Console.Clear();
                Console.WriteLine("Viewing past feedback...");
                // Implement the logic to view past feedback
                BackToMainMenu();
            }
        }

        public class EditDetailsOption : IMenu
        {
            public void Execute()
            {
                Console.Clear();
                Console.WriteLine("Editing details...");
                // Implement the logic to edit details
                BackToMainMenu();
            }
        }

        public class LogoutOption : IMenu
        {
            public void Execute()
            {
                Console.Clear();
                Console.WriteLine("Logging out...");
                Environment.Exit(0);
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
