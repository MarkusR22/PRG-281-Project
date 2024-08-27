using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace EventManagement
{
    internal class Participant : User, IParticipant
    {
        //EVENTS
        public delegate void RegisteredForEventHander(object source, RegisteredForEventArgs e);
        public event RegisteredForEventHander RegisteredForEvent;

        public delegate void UnregisteredForEventHander(object source, EventArgs e);
        public event UnregisteredForEventHander DeregisteredForEvent;
        //EVENTS

        //CONSTRUCTOR
        public Participant(int id, string userName, string password) : base(id, userName, password)
        {

        }
        //CONSTRUCTOR


        //PARTICIPANT USER MENU
        public enum ParticipantMenuOptions
        {
            Display_All_Upcoming_Events = 1,
            Register_For_Event,
            View_Registered_Events,
            Cancel_Registration,
            Submit_Feedback,
            Manage_Profile,
            Logout
        }
        //PARTICIPANT USER MENU

        //LOGOUT OVERRIDE FROM USER CLASS
        public override void Logout()
        {
            Console.Clear();
            Console.WriteLine("User logout successful!");
            Thread.Sleep(1500);
            Register_Login.DisplayMenu();
        }
        //LOGOUT OVERRIDE FROM USER CLASS


        //Displays when user needs to return to main menu
        public void DisplayBack()
        {
            Console.WriteLine();
            Console.Write("Press any key to go back.");
            Console.ReadKey();
            Thread.Sleep(500);
            Console.Clear();
        }


        //Override from participant interface class to display participant menu options and run participant methods.
        public override void DisplayMenu()
        {
            Console.Clear();

            //Instantiates object for event class
            NotifyService notify = new NotifyService();
            while (true)
            {
                Console.WriteLine($"Welcome {userName}:");

                //Displaying all options in ParticipantMenuOptions Enum
                foreach (ParticipantMenuOptions option in Enum.GetValues(typeof(ParticipantMenuOptions)))
                {
                    //Replaces underscore "_" with a space.
                    string optionName = option.ToString().Replace("_", " ");

                    Console.WriteLine($"{(int)option}. {optionName}");
                }

                Console.Write("Select an option (1-7): ");

                //ExceptionHandling.StringHandling(); is called to ensure correct input has been passed by user from ExceptionHandling class.
                string input = ExceptionHandling.StringHandling();

                //Enum.TryParse attempts to convert our string input to an enumeration value if this is correct
                //then the output will be sent to chosenOption 
                //Enum.IsDefined check if a value specified exists within the enum.
                if (Enum.TryParse(input, out ParticipantMenuOptions chosenOption) && Enum.IsDefined(typeof(ParticipantMenuOptions), chosenOption))
                {
                    switch (chosenOption)
                    {
                        //Runs methods to display all upcoming events and details about these events.
                        case ParticipantMenuOptions.Display_All_Upcoming_Events:
                            DisplayAllUpcoming();
                            ViewEventDetails();
                            break;

                        //This option allows a participant to register for any upcoming event.
                        case ParticipantMenuOptions.Register_For_Event:
                            RegisteredForEvent += notify.OnRegisteredForEvent;
                            RegisterForEvent();
                            RegisteredForEvent -= notify.OnRegisteredForEvent;
                            break;

                        //Views all the registered events a participant has registered for.
                        case ParticipantMenuOptions.View_Registered_Events:
                            ViewRegisteredEvents();
                            break;

                        //Removes a registered event from participant's account.
                        case ParticipantMenuOptions.Cancel_Registration:
                            DeregisteredForEvent += notify.OnDeregisteredForEvent;
                            CancelRegistration();
                            DeregisteredForEvent -= notify.OnDeregisteredForEvent;
                            break;

                        //Allows a participant to give feedback on past events they registered for
                        case ParticipantMenuOptions.Submit_Feedback:
                            SubmitFeedback();
                            break;

                        //Allows a participant to change their username and password
                        case ParticipantMenuOptions.Manage_Profile:
                            Register_Login.currentUser.ManageProfile();
                            DisplayMenu();
                            break;

                        //Logs out participant and returns to log in screen
                        case ParticipantMenuOptions.Logout:
                            Logout();
                            return;

                        //Error check to display to participant once they entered incorrect input.
                        default:
                            Console.Clear();
                            Console.WriteLine("Invalid option, please try again.");
                            break;
                    }
                }
                else
                {
                    //More error checking in case something else goes wrong.
                    Console.Clear();
                    Console.WriteLine("Invalid option, please try again.");
                }
            }
        }


        //ALL MENU METHODS


        //<---------------------------------------------------1-------------------------------------------------------->
        //This is a method called DisplayAllUpcoming that returns a List of (int eventId, string eventName)
        //It returns a list because this list is going to be called at other methods that want to display all upcoming events.
        public List<(int eventId, string eventName) > DisplayAllUpcoming()
        {
            Console.Clear();

            //Initialize an object for the return list.
            List<(int eventId, string eventName)> events = new List<(int eventId, string eventName)>();
            try
            {
                //Loading screen
                Console.WriteLine("Displaying Upcoming Events...");
                Thread.Sleep(1000);
                Console.Clear();

                //Database usage
                //A using block is defined to end all database tasks after they have finished running, for resource and security reasons
                //In the using statement an object is instantiated to connect to the database.
                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                {
                    //This is a requirement to communicate with the database it prepares the object "connection" to execute SQL commands.
                    connection.Open();
                    
                    //With the connection now open we can declare SQL statements for our object
                    //The command object allows the use of SQL via the SqlCommand class that takes two parameters
                    //The first is the SQL itself and the 2nd is the connection to the database above.
                    SqlCommand command = new SqlCommand("SELECT eventID, name FROM event WHERE status = 'upcoming'", connection);
                    
                    //This line of code executes the SQL statement above via the method ExecuteReader(); 
                    //It sends this command to the database server and executes giving us back the relevant data.
                    SqlDataReader reader = command.ExecuteReader();


                    Console.WriteLine("Upcoming Events:");
                    Console.WriteLine("====================");
                    int index = 1;

                    //reader.Read() goes through each row in our database that was filtered by the reader execution of the SQL query command
                    //it continues looping each row and returns a boolean till it finds the end and returns a false if no more rows are found.
                    while (reader.Read())
                    {
                        //Declaring variables equal to our database for eventId and eventName respectively
                        int eventId = (int)reader["eventID"];
                        string eventName = (string)reader["name"];

                        //These values then get added to the list we return to the method that other methods are able to use.
                        events.Add((eventId, eventName));

                        //Displays all upcoming events
                        Console.WriteLine($"{index}. {eventName}");

                        //increment index
                        index++;
                    }
                    Console.WriteLine("====================");
                }
            }
            catch (SqlException ex)
            {
                //Displays error message is database could not be reached.
                Console.WriteLine("An error occurred while displaying the events: " + ex.Message);
                
                //Allows participant to return to main menu.
                DisplayBack();
            }

            //returns the list back to method.
            return events;
        }

        public void ViewEventDetails()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                {
                    connection.Open();

                    // Query to select all upcoming events
                    SqlCommand command = new SqlCommand("SELECT * FROM event WHERE status = 'upcoming'", connection);
                    SqlDataReader reader = command.ExecuteReader();

                    // Check if there are any results
                    bool hasEvents = false;

                    // Loop through all rows in the result set
                    Console.WriteLine("All Upcoming Event Details:");
                    Console.WriteLine();
                    while (reader.Read())
                    {

                        hasEvents = true;
                        Console.WriteLine("====================");
                        Console.WriteLine($"Event ID: {reader["eventID"]}");
                        Console.WriteLine($"Name: {reader["name"]}");
                        Console.WriteLine($"Description: {reader["description"]}");
                        Console.WriteLine($"Date: {(DateTime)reader["date"]:yyyy-MM-dd}");
                        Console.WriteLine($"Location: {reader["location"]}");
                        Console.WriteLine($"Status: {reader["status"]}");
                        Console.WriteLine($"Ticket Price: {Math.Round((decimal)reader["ticket_price"], 2)}");
                        Console.WriteLine("====================");
                        Console.WriteLine();

                    }
                    DisplayBack();


                    if (!hasEvents)
                    {
                        Console.WriteLine("No upcoming events found.");
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("An error occurred while fetching event details: " + ex.Message);
            }
        }
        //<---------------------------------------------------1-------------------------------------------------------->


        //<---------------------------------------------------2-------------------------------------------------------->
        //Method that allows a participant to register for an event.
        public void RegisterForEvent()
        {
            Console.Clear();

            //List declaration of events that are available.
            List<(int eventId, string eventName)> availableEvents = new List<(int eventId, string eventName)>();

            //Database block using code
            using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
            {
                //Opens the connection so communication can be exchanged between program and database.
                connection.Open();

                // Retrieve all events
                List<(int eventId, string eventName)> allEvents = DisplayAllUpcoming();

                foreach (var evt in allEvents)
                {
                    // Check if the user is already registered for this event
                    SqlCommand checkRegistrationCommand = new SqlCommand(
                        "SELECT COUNT(*) FROM attendee_event WHERE userID = @userID AND eventID = @eventID",
                        connection);

                    //Creation of new SqlParameter objects with the name "@userID" and "@eventID" with values this.id and evt.eventId
                    checkRegistrationCommand.Parameters.AddWithValue("@userID", this.id);
                    checkRegistrationCommand.Parameters.AddWithValue("@eventID", evt.eventId);

                    //The following is executed once this runs
                    //SELECT COUNT(*) FROM attendee_event WHERE userID = @userID AND eventID = @eventID
                    //Meaning we set the value registrationCount to all instances where a participant is registered for an event
                    int registrationCount = (int)checkRegistrationCommand.ExecuteScalar();

                    //if a participant is not registered for an event we add to the list 
                    if (registrationCount == 0)
                    {
                        // If not registered, add to available events list
                        availableEvents.Add(evt);
                    }
                }
            }
            //If registrationCount is != 0 then we add no events meaning the participant is already registered for all events
            //This then checks the list to see if any event has been added and since none has we execute this block of code.
            if (availableEvents.Count == 0)
            {
                Console.WriteLine("You have already registered for all upcoming events.");

                //Allows user to return to main menu
                DisplayBack();
            }
            else
            {
                // Display available events with the new heading
                Console.WriteLine();
                Console.WriteLine("Events that you can register for:");
                Console.WriteLine("====================");
                for (int i = 0; i < availableEvents.Count; i++)
                {
                    //Displays all events a participant is able to register for
                    Console.WriteLine($"{i + 1}. {availableEvents[i].eventName}");
                }
                Console.WriteLine("====================");
                Console.WriteLine();
                Console.WriteLine("(0: Back)");
                Console.Write("Enter the number corresponding to the event you want to register for: ");

                //Calling of ExceptionHandling class to see if input is correctly inputted
                string userInput = ExceptionHandling.StringHandling();


                //Changes userInput string to int selectedIndex and then another check to make user select between 1 and the max events to choose.
                if (int.TryParse(userInput, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= availableEvents.Count)
                {
                    //uses list to put eventId equal to
                    int eventId = availableEvents[selectedIndex - 1].eventId;

                    //Calls method to register participant 
                    RegisterForSelectedEvent(eventId);

                }
                else if (userInput == "0")
                {
                    //Returns participant to main menu
                    DisplayBack();
                }
                else
                {
                    //Error handling 
                    Console.WriteLine("Invalid selection. Please enter a number corresponding to the event.");
                    
                    //Returns participant to main menu.
                    DisplayBack();
                }

            }


        }

        //Register participant to an event
        //Requires eventId parameter to add to database
        private void RegisterForSelectedEvent(int eventId)
        {
            try
            {
                //Database connection
                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                {
                    //Opens communication for database
                    connection.Open();

                    // Get event details
                    SqlCommand getEventCommand = new SqlCommand("SELECT name, YEAR(date) FROM event WHERE eventID = @eventID", connection);
                    getEventCommand.Parameters.AddWithValue("@eventID", eventId);
                    SqlDataReader reader = getEventCommand.ExecuteReader();

                    if (reader.Read())
                    {
                        //Declares eventName, eventYear 
                        //Event name is index 0 and event Year is index 1
                        string eventName = reader.GetString(0);
                        int eventYear = reader.GetInt32(1);
                        reader.Close();

                        // Generate the entry code
                        string entryCode = GenerateEntryCode(eventName, eventYear, connection, eventId);

                        // Register for the event
                        SqlCommand command = new SqlCommand("INSERT INTO attendee_event (userID, eventID, entry_code) VALUES (@userID, @eventID, @entryCode)", connection);
                        command.Parameters.AddWithValue("@userID", this.id);
                        command.Parameters.AddWithValue("@eventID", eventId);
                        command.Parameters.AddWithValue("@entryCode", entryCode);

                        //ExecuteNonQuery() is a method that does not return a result set.
                        int rowsAffected = command.ExecuteNonQuery();

                        //rowsAffected should return 1 if this is true then the following block will run
                        if (rowsAffected > 0)
                        {
                            //Notifies subscriber for event
                            OnRegisteredForEvent(entryCode);
                            DisplayBack();
                        }
                        else
                        {
                            //Error checking if anything went wrong with the insert.
                            Console.WriteLine("Failed to register for the event.");
                            Console.Clear();
                        }
                    }
                    else
                    {
                        //More error checking to display if reader does not read to the database.
                        Console.WriteLine("Event not found.");
                    }
                }
            }
            catch (SqlException ex)
            {
                //Error checking 
                Console.WriteLine("An error occurred while registering for the event: " + ex.Message);
            }
        }

        //Invoking event OnRegisteredForEvent
        public void OnRegisteredForEvent(string entryCode)
        {
            RegisteredForEvent?.Invoke(this, new RegisteredForEventArgs() { entryCode = entryCode});
        }

        private string GenerateEntryCode(string eventName, int eventYear, SqlConnection connection, int eventId)
        {
            //Entry code example
            //EventName: Tree House Designing, EventYear: 2024, Registration: 01
            //Entry code = TH2020401

            // Extract the first letter of the first two words in the event name
            string[] words = eventName.Split(' ');
            string initials = (words.Length > 1) ? $"{words[0][0]}{words[1][0]}" : $"{words[0][0]}";

            // Generate the base part of the code
            string baseCode = $"{initials.ToUpper()}{eventYear}";

            // Get the current maximum entry code for this event
            SqlCommand getMaxCodeCommand = new SqlCommand(
                "SELECT TOP 1 entry_code FROM attendee_event WHERE eventID = @eventID ORDER BY entry_code DESC", connection);
            getMaxCodeCommand.Parameters.AddWithValue("@eventID", eventId);

            string maxCode = (string)getMaxCodeCommand.ExecuteScalar();
            int increment = 1;

            if (maxCode != null)
            {
                // Extract the numeric part and increment it
                string numericPart = maxCode.Substring(baseCode.Length);
                if (int.TryParse(numericPart, out int lastNumber))
                {
                    increment = lastNumber + 1;
                }
            }

            // Format the increment with leading zeros if necessary
            string incrementPart = increment.ToString("D2"); // Ensure it has at least 2 digits

            // Generate the final entry code
            string entryCode = $"{baseCode}{incrementPart}";

            // Check the length of the entry code to ensure it fits in the column
            SqlCommand getColumnLengthCommand = new SqlCommand(
                "SELECT CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'attendee_event' AND COLUMN_NAME = 'entry_code'", connection);
            int maxLength = (int)getColumnLengthCommand.ExecuteScalar();

            if (entryCode.Length > maxLength)
            {
                entryCode = entryCode.Substring(0, maxLength);
            }

            return entryCode;
        }
        //<---------------------------------------------------2-------------------------------------------------------->

        //<---------------------------------------------------3-------------------------------------------------------->
        public void ViewRegisteredEvents()
        {
            try
            {
                //Database using block
                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                {
                    //Opens database communication
                    connection.Open();
                    Console.Clear();
                    Console.WriteLine("Displaying all events that the user has registered for...");
                    Thread.Sleep(1000);
                    Console.Clear();

                    SqlCommand command = new SqlCommand(
                        "SELECT e.eventID, e.name, e.date, e.location, ae.entry_code, e.ticket_price " +
                        "FROM event e INNER JOIN attendee_event ae ON e.eventID = ae.eventID " +
                        "WHERE ae.userID = @userID", connection);

                    //Adds participant's userID to attendee_event making them registered
                    command.Parameters.AddWithValue("@userID", this.id);

                    SqlDataReader reader = command.ExecuteReader();

                    Console.WriteLine("Registered Events:");
                    Console.WriteLine("====================");
                    while (reader.Read())
                    {
                        //Displays all events participant is registered for.
                        Console.WriteLine($"Name: {reader["name"]}\n" +
                                          $"Date: {(DateTime)reader["date"]:yyyy-MM-dd}\n" +
                                          $"Location: {reader["location"]}\n" +
                                          $"Entry Code: {reader["entry_code"]}\n"+
                                          $"Ticket Price: {Math.Round((decimal)reader["ticket_price"], 2)}\n"
                                          );
                    }
                    Console.WriteLine("====================");
                    DisplayBack();
                }
            }
            catch (SqlException ex)
            {
                //Error checking
                Console.WriteLine("An error occurred while fetching registered events: " + ex.Message);
            }
        }
        //<---------------------------------------------------3-------------------------------------------------------->

        //<---------------------------------------------------4-------------------------------------------------------->
        public void CancelRegistration()
        {

            Console.Clear();
            Console.WriteLine("Displaying All events the user can cancel registration for...");
            Thread.Sleep(1000);
            Console.Clear();
            // Display upcoming events the participant is registered for
            List<int> eventIds = DisplayRegisteredUpcomingEvents();


            if (eventIds.Count == 0)
            {
                Console.WriteLine("You are not registered for any upcoming events.");
                return;
            }

            Console.Write("Enter the number of the event you wish to cancel registration for: ");
            if (!int.TryParse(Console.ReadLine(), out int selectedIndex) || selectedIndex < 1 || selectedIndex > eventIds.Count)
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                DisplayBack();
                return;
            }

            int eventId = eventIds[selectedIndex - 1]; //putting variable equal to eventId in the list

            try
            {
                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("DELETE FROM attendee_event WHERE userID = @userID AND eventID = @eventID", connection);
                    command.Parameters.AddWithValue("@userID", this.id);
                    command.Parameters.AddWithValue("@eventID", eventId);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        //Event trigger
                        OnUnregisteredForEvent();
                        DisplayBack();
                    }
                    else
                    {
                        Console.WriteLine("Failed to cancel registration. Please make sure you are registered for this event.");
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("An error occurred while canceling your registration: " + ex.Message);
            }
        }

        public void OnUnregisteredForEvent()
        {
            DeregisteredForEvent?.Invoke(this, EventArgs.Empty);
        }

        // Helper method to display upcoming events the user is registered for and return their IDs
        private List<int> DisplayRegisteredUpcomingEvents()
        {
            List<int> eventIds = new List<int>();

            try
            {
                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(
                        "SELECT e.eventID, e.name, e.date, e.location " +
                        "FROM event e JOIN attendee_event a ON e.eventID = a.eventID " +
                        "WHERE a.userID = @userID AND e.status = 'upcoming'",
                        connection);
                    command.Parameters.AddWithValue("@userID", this.id);

                    SqlDataReader reader = command.ExecuteReader();

                    Console.WriteLine("You are currently registered for the following events:");
                    Console.WriteLine();
                    int index = 1;
                    while (reader.Read())
                    {
                        Console.WriteLine("====================");
                        Console.WriteLine($"{index}. Event ID: {reader["eventID"]}");
                        Console.WriteLine($"   Name: {reader["name"]}");
                        Console.WriteLine($"   Date: {(DateTime)reader["date"]:yyyy-MM-dd}");
                        Console.WriteLine($"   Location: {reader["location"]}");
                        Console.WriteLine("====================");
                        Console.WriteLine();
                        eventIds.Add((int)reader["eventID"]);
                        index++;


                    }

                }

            }
            catch (SqlException ex)
            {
                Console.WriteLine("An error occurred while retrieving registered upcoming events: " + ex.Message);

            }

            return eventIds;
        }
        //<---------------------------------------------------4-------------------------------------------------------->


        //<---------------------------------------------------5-------------------------------------------------------->

        public void SubmitFeedback()
        {
            Console.Clear();
            Console.WriteLine("Displaying Past Events...");
            Thread.Sleep(1000);
            Console.Clear();

            // Check if there are any ended events the user is registered for
            if (!HasRegisteredEndedEvents())
            {
                Console.WriteLine("You are not registered for any events that have ended.");
                DisplayBack();
                return;
            }

            // Display the user's registered events that have ended
            ViewUserRegisteredEndedEvents();
            Console.Write("Please select the correct number to add feedback to: ");
            if (!int.TryParse(Console.ReadLine(), out int selectedIndex))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                DisplayBack();
                return;
            }

            // Map selected index to event ID
            int eventId = GetEventIdByIndex(selectedIndex);
            if (eventId == -1)
            {
                Console.WriteLine("Invalid event selection. Please try again.");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                {
                    connection.Open();

                    // Check if the participant is registered for the event
                    SqlCommand checkRegistrationCommand = new SqlCommand("SELECT COUNT(*) FROM attendee_event WHERE userID = @userID AND eventID = @eventID", connection);
                    checkRegistrationCommand.Parameters.AddWithValue("@userID", this.id);
                    checkRegistrationCommand.Parameters.AddWithValue("@eventID", eventId);

                    int registrationCount = (int)checkRegistrationCommand.ExecuteScalar();

                    if (registrationCount == 0)
                    {
                        Console.WriteLine("You are not registered for this event.");
                        return;
                    }

                    // Check if the event has ended
                    SqlCommand checkEventCommand = new SqlCommand("SELECT status FROM event WHERE eventID = @eventID", connection);
                    checkEventCommand.Parameters.AddWithValue("@eventID", eventId);
                    string eventStatus = (string)checkEventCommand.ExecuteScalar();

                    if (eventStatus.ToLower() != "ended")
                    {
                        Console.WriteLine("You cannot submit feedback for an event that has not ended.");
                        return;
                    }

                    // Collect feedback and rating from the participant
                    Console.WriteLine();
                    Console.WriteLine("====================");
                    Console.Write("Enter your feedback: ");
                    string feedbackComment = ExceptionHandling.StringHandling();

                    Console.Write("Enter your rating (1 to 5): ");
                    if (!int.TryParse(Console.ReadLine(), out int rating) || rating < 1 || rating > 5)
                    {
                        Console.WriteLine("Invalid rating. Please enter a number between 1 and 5.");
                        DisplayBack();
                        return;
                    }

                    // Insert feedback into the database
                    SqlCommand insertFeedbackCommand = new SqlCommand("INSERT INTO feedback (eventID, comment, rating) VALUES (@eventID, @comment, @rating)", connection);
                    insertFeedbackCommand.Parameters.AddWithValue("@eventID", eventId);
                    insertFeedbackCommand.Parameters.AddWithValue("@comment", feedbackComment);
                    insertFeedbackCommand.Parameters.AddWithValue("@rating", rating);

                    int rowsAffected = insertFeedbackCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("====================");
                        Console.WriteLine();
                        Console.WriteLine("Thank you for your feedback!");
                        DisplayBack();
                    }
                    else
                    {
                        Console.WriteLine("Failed to submit feedback. Please try again.");
                        DisplayBack();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("An error occurred while submitting feedback: " + ex.Message);
            }
        }

        // Method to check if the participant has any registered ended events
        private bool HasRegisteredEndedEvents()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(
                        "SELECT COUNT(*) FROM attendee_event a JOIN event e ON a.eventID = e.eventID WHERE a.userID = @userID AND e.status = 'ended'",
                        connection);
                    command.Parameters.AddWithValue("@userID", this.id);
                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("An error occurred while checking registered ended events: " + ex.Message);
            }
            return false;
        }


        // Helper method to get the event ID by index from registered ended events
        private int GetEventIdByIndex(int index)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                {
                    connection.Open();

                    // Update query to specify table alias for eventID
                    SqlCommand command = new SqlCommand(
                        "SELECT e.eventID FROM attendee_event a JOIN event e ON a.eventID = e.eventID WHERE a.userID = @userID AND e.status = 'ended'",
                        connection);
                    command.Parameters.AddWithValue("@userID", this.id);
                    SqlDataReader reader = command.ExecuteReader();

                    int currentIndex = 1;
                    while (reader.Read())
                    {
                        if (currentIndex == index)
                        {
                            return reader.GetInt32(0); // Retrieve eventID from the first column
                        }
                        currentIndex++;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("An error occurred while retrieving event IDs: " + ex.Message);
            }
            return -1; // Return -1 if the index is invalid
        }


        // Method to display only ended events for which the participant is registered
        private void ViewUserRegisteredEndedEvents()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                {
                    connection.Open();

                    // Update query to specify table alias for columns
                    SqlCommand command = new SqlCommand(
                        "SELECT e.eventID, e.name, e.description, e.date, e.location, e.status, e.ticket_price " +
                        "FROM event e JOIN attendee_event a ON e.eventID = a.eventID " +
                        "WHERE a.userID = @userID AND e.status = 'ended'",
                        connection);
                    command.Parameters.AddWithValue("@userID", this.id);
                    SqlDataReader reader = command.ExecuteReader();

                    Console.WriteLine("Ended events you can provide feedback for:");
                    Console.WriteLine("====================");
                    Console.WriteLine();
                    int index = 1;
                    while (reader.Read())
                    {
                        Console.WriteLine("====================");
                        Console.WriteLine($"{index}. Event ID: {reader["eventID"]}");
                        Console.WriteLine($"   Name: {reader["name"]}");
                        Console.WriteLine($"   Description: {reader["description"]}");
                        Console.WriteLine($"   Date: {(DateTime)reader["date"]:yyyy-MM-dd}");
                        Console.WriteLine($"   Location: {reader["location"]}");
                        Console.WriteLine($"   Status: {reader["status"]}");
                        Console.WriteLine($"   Ticket Price: {reader["ticket_price"]}");
                        Console.WriteLine("====================");
                        Console.WriteLine();
                        index++;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("An error occurred while fetching events: " + ex.Message);
            }
        }
        //<---------------------------------------------------5-------------------------------------------------------->
        //ALL MENU METHODS

    }
}
