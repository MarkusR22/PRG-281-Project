﻿using System;
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
                        case ParticipantMenuOptions.Search_Display:

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
    }
}
