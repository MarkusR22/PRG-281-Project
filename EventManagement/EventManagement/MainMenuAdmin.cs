﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement
{
    internal class MainMenuAdmin
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
            while (true)
            {
                Console.WriteLine("Admin Menu:");

                foreach (AdminMenuOptions option in Enum.GetValues(typeof(AdminMenuOptions)))
                {
                    string optionName = option.ToString().Replace("_", " ");
                    Console.WriteLine($"{(int)option}. {optionName}");
                }

                Console.Write("Select an option (1-6): ");
                string input = Console.ReadLine().Trim();
                IMenuOption selectedOption = null;

                if (Enum.TryParse(input, out AdminMenuOptions chosenOption) && Enum.IsDefined(typeof(AdminMenuOptions), chosenOption))
                {
                    switch (chosenOption)
                    {
                        case AdminMenuOptions.View_Upcoming_Events:
                            selectedOption = new ViewUpcomingEventsOption();
                            break;
                        case AdminMenuOptions.Create_New_Event:
                            selectedOption = new CreateNewEventOption();
                            break;
                        case AdminMenuOptions.Register_New_Organizer:
                            selectedOption = new RegisterNewOrganizerOption();
                            break;
                        case AdminMenuOptions.Past_Feedback:
                            selectedOption = new PastFeedbackOption();
                            break;
                        case AdminMenuOptions.Edit_Details:
                            selectedOption = new EditDetailsOption();
                            break;
                        case AdminMenuOptions.Log_out:
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

        public class ViewUpcomingEventsOption : IMenuOption
        {
            public void Execute()
            {
                Console.Clear();
                Console.WriteLine("Viewing upcoming events...");
                // Implement the logic to view upcoming events
                BackToMainMenu();
            }
        }

        public class CreateNewEventOption : IMenuOption
        {
            public void Execute()
            {
                Console.Clear();
                Console.WriteLine("Creating a new event...");
                // Implement the logic to create a new event
                BackToMainMenu();
            }
        }

        public class RegisterNewOrganizerOption : IMenuOption
        {
            public void Execute()
            {
                Console.Clear();
                Console.WriteLine("Registering a new organizer...");
                // Implement the logic to register a new organizer
                BackToMainMenu();
            }
        }

        public class PastFeedbackOption : IMenuOption
        {
            public void Execute()
            {
                Console.Clear();
                Console.WriteLine("Viewing past feedback...");
                // Implement the logic to view past feedback
                BackToMainMenu();
            }
        }

        public class EditDetailsOption : IMenuOption
        {
            public void Execute()
            {
                Console.Clear();
                Console.WriteLine("Editing details...");
                // Implement the logic to edit details
                BackToMainMenu();
            }
        }

        public class LogoutOption : IMenuOption
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
            Console.WriteLine("Press any key to return to the admin menu...");
            Console.ReadKey();
            Console.Clear();
        }

    }
}
