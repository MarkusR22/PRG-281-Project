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
        public delegate void RegisteredForEventHander(object source, RegisteredForEventArgs e);
        public event RegisteredForEventHander RegisteredForEvent;

        public delegate void UnregisteredForEventHander(object source, EventArgs e);
        public event UnregisteredForEventHander UnregisteredForEvent;

        
        

        public Participant(int id, string userName, string password) : base(id, userName, password)
        {

        }

        public enum ParticipantMenuOptions
        {
            Display_All_Upcoming_Events = 1,
            View_Event_Details,
            Register_For_Event,
            View_Registered_Events,
            Cancel_Registration,
            Submit_Feedback,
            Manage_Profile,
            Logout
        }

        public override void Logout()
        {
            Console.Clear();
            Console.WriteLine("User logout successful!");
            Thread.Sleep(1500);
            Register_Login.DisplayMenu();
        }

        public void DisplayBack()
        {
            Console.WriteLine();
            Console.Write("Press any key to go back.");
            Console.ReadKey();
            Thread.Sleep(500);
            Console.Clear();
        }

        public override void DisplayMenu()
        {
            Console.Clear();
            NotifyService notify = new NotifyService();
            while (true)
            {
                Console.WriteLine($"Welcome {userName}:");

                foreach (ParticipantMenuOptions option in Enum.GetValues(typeof(ParticipantMenuOptions)))
                {
                    string optionName = option.ToString().Replace("_", " ");
                    Console.WriteLine($"{(int)option}. {optionName}");
                }

                Console.Write("Select an option (1-8): ");
                string input = ExceptionHandling.StringHandling();

                if (Enum.TryParse(input, out ParticipantMenuOptions chosenOption) && Enum.IsDefined(typeof(ParticipantMenuOptions), chosenOption))
                {
                    switch (chosenOption)
                    {
                        case ParticipantMenuOptions.Display_All_Upcoming_Events:
                            SearchEvents();
                            break;

                        case ParticipantMenuOptions.View_Event_Details:
                            ViewEventDetails();
                            break;

                        case ParticipantMenuOptions.Register_For_Event:
                            RegisteredForEvent += notify.OnRegisteredForEvent;
                            RegisterForEvent();
                            RegisteredForEvent -= notify.OnRegisteredForEvent;
                            break;

                        case ParticipantMenuOptions.View_Registered_Events:
                            ViewRegisteredEvents();
                            break;

                        case ParticipantMenuOptions.Cancel_Registration:
                            UnregisteredForEvent += notify.OnUnregisteredForEvent;
                            CancelRegistration();
                            UnregisteredForEvent -= notify.OnUnregisteredForEvent;
                            break;

                        case ParticipantMenuOptions.Submit_Feedback:
                            SubmitFeedback();
                            break;
                        case ParticipantMenuOptions.Manage_Profile:
                            Register_Login.currentUser.ManageProfile();
                            DisplayMenu();
                            break;

                        case ParticipantMenuOptions.Logout:
                            Logout();
                            return;

                        default:
                            Console.Clear();
                            Console.WriteLine("Invalid option, please try again.");
                            break;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid option, please try again.");
                }
            }
        }


        //NEW METHODS FOR NEW MENU
        public List<(int eventId, string eventName) > SearchEvents(bool showExitMessage = true)
        {
            Console.Clear();
            List<(int eventId, string eventName)> events = new List<(int eventId, string eventName)>();
            try
            {
                Console.WriteLine("Displaying Upcoming Events...");
                Thread.Sleep(1000);
                Console.Clear();
                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("SELECT eventID, name FROM event WHERE status = 'upcoming'", connection);
                    SqlDataReader reader = command.ExecuteReader();

                    Console.WriteLine("Upcoming Events:");
                    Console.WriteLine("====================");
                    int index = 1;
                    while (reader.Read())
                    {
                        int eventId = (int)reader["eventID"];
                        string eventName = (string)reader["name"];
                        events.Add((eventId, eventName));

                        Console.WriteLine($"{index}. {eventName}");
                        index++;
                    }
                    Console.WriteLine("====================");

                    if (showExitMessage)
                    {
                        DisplayBack();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("An error occurred while dislpaying the events: " + ex.Message);
                DisplayBack();
            }

            return events;
        }



        public void RegisterForEvent()
        {
            Console.Clear();
            List<(int eventId, string eventName)> availableEvents = new List<(int eventId, string eventName)>();

            using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
            {
                connection.Open();

                // Retrieve all events
                List<(int eventId, string eventName)> allEvents = SearchEvents(showExitMessage: false);

                foreach (var evt in allEvents)
                {
                    // Check if the user is already registered for this event
                    SqlCommand checkRegistrationCommand = new SqlCommand(
                        "SELECT COUNT(*) FROM attendee_event WHERE userID = @userID AND eventID = @eventID",
                        connection);
                    checkRegistrationCommand.Parameters.AddWithValue("@userID", this.id);
                    checkRegistrationCommand.Parameters.AddWithValue("@eventID", evt.eventId);

                    int registrationCount = (int)checkRegistrationCommand.ExecuteScalar();

                    if (registrationCount == 0)
                    {
                        // If not registered, add to available events list
                        availableEvents.Add(evt);
                    }
                }
            }

            if (availableEvents.Count == 0)
            {
                Console.WriteLine("You have already registered for all upcoming events.");
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
                    Console.WriteLine($"{i + 1}. {availableEvents[i].eventName}");
                }
                Console.WriteLine("====================");
                Console.WriteLine();
                Console.WriteLine("(0: Back)");
                Console.Write("Enter the number corresponding to the event you want to register for: ");
                string userInput = ExceptionHandling.StringHandling();

                if (int.TryParse(userInput, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= availableEvents.Count)
                {
                    int eventId = availableEvents[selectedIndex - 1].eventId;
                    RegisterForSelectedEvent(eventId);

                }
                else if (userInput == "0")
                {
                    DisplayBack();
                }
                else
                {
                    Console.WriteLine("Invalid selection. Please enter a number corresponding to the event.");
                    DisplayBack();
                }

            }


        }

        private void RegisterForSelectedEvent(int eventId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                {
                    connection.Open();

                    // Check if the user is already registered for the event
                    SqlCommand checkRegistrationCommand = new SqlCommand("SELECT COUNT(*) FROM attendee_event WHERE userID = @userID AND eventID = @eventID", connection);
                    checkRegistrationCommand.Parameters.AddWithValue("@userID", this.id);
                    checkRegistrationCommand.Parameters.AddWithValue("@eventID", eventId);

                    int registrationCount = (int)checkRegistrationCommand.ExecuteScalar();

                    if (registrationCount > 0)
                    {
                        Console.WriteLine("You are already registered for this event.");
                        DisplayBack();
                        return; // Exit the method early
                    }

                    // Get event details
                    SqlCommand getEventCommand = new SqlCommand("SELECT name, YEAR(date) FROM event WHERE eventID = @eventID", connection);
                    getEventCommand.Parameters.AddWithValue("@eventID", eventId);
                    SqlDataReader reader = getEventCommand.ExecuteReader();

                    if (reader.Read())
                    {
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

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            //Notifies subscriber for event
                            OnRegisteredForEvent(entryCode);
                            DisplayBack();
                        }
                        else
                        {
                            Console.WriteLine("Failed to register for the event.");
                            Console.Clear();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Event not found.");
                    }
                }
            }
            catch (SqlException ex)
            {
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

        public void ViewRegisteredEvents()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                {
                    connection.Open();
                    Console.Clear();
                    Console.WriteLine("Displaying all events that the user has registered for...");
                    Thread.Sleep(1000);
                    Console.Clear();

                    SqlCommand command = new SqlCommand(
                        "SELECT e.eventID, e.name, e.date, e.location, ae.entry_code " +
                        "FROM event e INNER JOIN attendee_event ae ON e.eventID = ae.eventID " +
                        "WHERE ae.userID = @userID", connection);
                    command.Parameters.AddWithValue("@userID", this.id);

                    SqlDataReader reader = command.ExecuteReader();

                    Console.WriteLine("Registered Events:");
                    Console.WriteLine("====================");
                    while (reader.Read())
                    {
                        Console.WriteLine($"Name: {reader["name"]}\nDate: {((DateTime)reader["date"]).ToString("yyyy-MM-dd")}\nLocation: {reader["location"]}\nEntry Code: {reader["entry_code"]}");
                    }
                    Console.WriteLine("====================");
                    DisplayBack();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("An error occurred while fetching registered events: " + ex.Message);
            }
        }


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
            int selectedIndex;
            if (!int.TryParse(Console.ReadLine(), out selectedIndex))
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

                    // Check if the user is registered for the event
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

                    // Collect feedback and rating from the user
                    Console.WriteLine();
                    Console.WriteLine("====================");
                    Console.Write("Enter your feedback: ");
                    string feedbackComment = ExceptionHandling.StringHandling();

                    Console.Write("Enter your rating (1 to 5): ");
                    int rating;
                    if (!int.TryParse(Console.ReadLine(), out rating) || rating < 1 || rating > 5)
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

        // Method to check if the user has any registered ended events
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


        // Method to display only ended events for which the user is registered
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
                        Console.WriteLine($"   Date: {((DateTime)reader["date"]).ToString("yyyy-MM-dd")}");
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


        public void CancelRegistration()
        {
            // Display upcoming events the user is registered for
            Console.Clear();
            Console.WriteLine("Displaying All events the user can cancel registration for...");
            Thread.Sleep(1000);
            Console.Clear();
            List<int> eventIds = DisplayRegisteredUpcomingEvents();
            

            if (eventIds.Count == 0)
            {
                Console.WriteLine("You are not registered for any upcoming events.");
                return;
            }

            Console.Write("Enter the number of the event you wish to cancel registration for: ");
            int selectedIndex;
            if (!int.TryParse(Console.ReadLine(), out selectedIndex) || selectedIndex < 1 || selectedIndex > eventIds.Count)
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                DisplayBack();
                return;
            }

            int eventId = eventIds[selectedIndex - 1]; // Adjust index for zero-based list

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
            UnregisteredForEvent?.Invoke(this, EventArgs.Empty);
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
                        Console.WriteLine($"   Date: {((DateTime)reader["date"]).ToString("yyyy-MM-dd")}");
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



        public void ViewEventDetails()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                {
                    connection.Open();
                    Console.Clear();
                    Console.WriteLine("Viewing All Upcoming Event Details...");
                    Thread.Sleep(1000);
                    Console.Clear();
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
                        Console.WriteLine($"Date: {((DateTime)reader["date"]).ToString("yyyy-MM-dd")}");
                        Console.WriteLine($"Location: {reader["location"]}");
                        Console.WriteLine($"Status: {reader["status"]}");
                        Console.WriteLine($"Ticket Price: {reader["ticket_price"]}");
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


        //NEW METHODS FOR NEW MENU
    }
}
