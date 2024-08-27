using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventManagement
{

    public class Admin : User, IEventManager, IAdmin
    {
        //Create an instance of the event manager to allow the admin to perform actions on Events
        EventManager eventManager = new EventManager();

        public delegate void EventApprovedHandler(object sender, EventArgs e);
        public event EventApprovedHandler EventApproved;

        public delegate void EventCancelledHandler(object sender, EventArgs e);
        public event EventCancelledHandler EventCancelled;
        
        
        public Admin(int id, string userName, string password) : base(id, userName, password)
        {

        }

        public enum AdminMenuOptions
        {
            View_Upcoming_Events = 1,
            Create_New_Event,
            Approve_Events,
            Cancel_Event,
            Register_New_Organizer,
            View_Past_Feedback,
            View_All_Users,
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


        //Main Menu For Admin
        public override void DisplayMenu()
        {
            NotifyService notify = new NotifyService();

            Console.Clear();
            Console.WriteLine($"Welcome Admin {userName}:");

            foreach (AdminMenuOptions option in Enum.GetValues(typeof(AdminMenuOptions)))
            {
                string optionName = option.ToString().Replace("_", " ");
                Console.WriteLine($"{(int)option}. {optionName}");
            }

            Console.Write("Select an option (1-9): ");
            string input = ExceptionHandling.StringHandling();


            if (Enum.TryParse(input, out AdminMenuOptions chosenOption) && Enum.IsDefined(typeof(AdminMenuOptions), chosenOption))
            {
                switch (chosenOption)
                {
                    case AdminMenuOptions.View_Upcoming_Events:
                        Console.Clear();
                        Thread displayingUpcomingEvents = new Thread(eventManager.DisplayUpcommingEvents);
                        Thread backToMainMenu = new Thread(BackToMainMenu);

                        displayingUpcomingEvents.Start();
                        displayingUpcomingEvents.Join();
                        backToMainMenu.Start();
                        backToMainMenu.Join();
                        //eventManager.DisplayUpcommingEvents();
                        //BackToMainMenu();
                     

                        break;
                    case AdminMenuOptions.Create_New_Event:
                        Console.Clear();
                        Thread CreateEventThread = new Thread(CreateEvent);
                        backToMainMenu = new Thread(BackToMainMenu);

                        CreateEventThread.Start();
                        CreateEventThread.Join();

                        backToMainMenu.Start();
                        backToMainMenu.Join();

                        break;
                    case AdminMenuOptions.Approve_Events:
                        EventApproved += notify.OnEventApproved;
                        ApproveEvent();
                        EventApproved -= notify.OnEventApproved;

                        break;
                    case AdminMenuOptions.Cancel_Event:
                        EventCancelled += notify.OnEventCancelled;
                        CancelEvent();
                        EventCancelled -= notify.OnEventCancelled;

                        break;
                    case AdminMenuOptions.Register_New_Organizer:
                        RegisterNewOrganizer();

                        break;
                    case AdminMenuOptions.View_Past_Feedback:
                        ViewFeedback();
                        break;
                    case AdminMenuOptions.View_All_Users:
                        DisplayAllUsers();
                        break;

                    case AdminMenuOptions.Edit_Details:
                        Register_Login.currentUser.ManageProfile();
                        BackToMainMenu();
                        break;
                    case AdminMenuOptions.Log_out:
                        notify = null;
                        Logout();

                        break;
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Invalid option, please try again.");
            }

        }

        //Calls the CreateEvent method from the event manager class using the event manager object
        public void CreateEvent()
        {
            Console.Clear();
            Console.WriteLine("Create An Event\n==================================");
            eventManager.CreateEvent();
        }

        //Display all events with status "pending" to allow the admin to approve the event
        public void ApproveEvent()
        {
            Console.Clear();
            List<int> eventIds = new List<int>();

            try
            {
                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                {
                    connection.Open();

                    // Query to get events with status 'pending'
                    string query = "SELECT eventID, name FROM dbo.[event] WHERE status = 'pending'";
                    SqlCommand command = new SqlCommand(query, connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        int index = 1;
                        Console.WriteLine("Currently Pending Events:");
                        Console.WriteLine("==================================");

                        while (reader.Read())
                        {
                            int eventId = (int)reader["eventID"];
                            string eventName = reader["name"].ToString();

                            // Display event with index
                            Console.WriteLine($"{index}. {eventName}");
                            eventIds.Add(eventId);
                            index++;
                        }
                        Console.WriteLine("==================================");

                        if (eventIds.Count == 0)
                        {
                            Console.WriteLine("No pending events found.");

                        }
                    }
                }

                // Ask the user to select an event from list
                Console.WriteLine("\nSelect an event by number:\n(0: Back)");
                int selectedEventIndex = ExceptionHandling.IntHandling();

                if (selectedEventIndex < 0 || selectedEventIndex > eventIds.Count)
                {
                    Console.WriteLine("Invalid selection.");
                    return;
                } else if(selectedEventIndex == 0)
                {
                    DisplayMenu();
                    return;
                }

                int selectedEventId = eventIds[selectedEventIndex - 1];

                // Ask the user what they want to do with the selected event
                Console.WriteLine("\n1. Approve");
                Console.WriteLine("2. Deny");
                int choice = ExceptionHandling.IntHandling();

                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                {
                    connection.Open();

                    if (choice == 1)
                    {
                        // Approve the event (change status to 'upcoming')
                        string approveQuery = "UPDATE dbo.[event] SET status = 'upcoming' WHERE eventID = @EventID";
                        SqlCommand approveCommand = new SqlCommand(approveQuery, connection);
                        approveCommand.Parameters.AddWithValue("@EventID", selectedEventId);

                        int rowsAffected = approveCommand.ExecuteNonQuery();

                        //Event triggers when new event is approved then inoforms organizer the event was approved
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Event approved successfully.");
                            OnEventApproved(EventArgs.Empty);
                        }
                        else
                        {
                            Console.WriteLine("Error: Failed to approve the event.");
                        }
                    }
                    else if (choice == 2)
                    {
                        // Deny the event and removes from table, then informs the organizer of the event
                        string denyQuery = "DELETE FROM dbo.[event] WHERE eventID = @EventID";
                        SqlCommand denyCommand = new SqlCommand(denyQuery, connection);
                        denyCommand.Parameters.AddWithValue("@EventID", selectedEventId);

                        int rowsAffected = denyCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Event denied and removed successfully.");
                            OnEventCancelled(EventArgs.Empty);
                        }
                        else
                        {
                            Console.WriteLine("Failed to remove the event.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid action. ");
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("An error occurred while processing events: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
            }
            finally
            {
                BackToMainMenu();
            }
        }

        public void OnEventApproved(EventArgs e)
        {
            EventApproved?.Invoke(null, e);
        }

        //Display all events with status "upcoming" to allow the admin to cancel the event if neccesary
        public void CancelEvent()
        {
            Console.Clear();
            try
            {

                {
                    //Retrieve the list of upcoming events
                    List<Event> events = eventManager.GetEvents();
                    List<Event> upcomingEvents = events.Where(e => e.Status == "upcoming").ToList();

                    if (upcomingEvents.Count == 0)
                    {
                        Console.WriteLine("No upcoming events to cancel.");
                        return;
                    }

                    //Display the list of upcoming events
                    Console.WriteLine("Upcoming Events:");
                    Console.WriteLine("==================================");
                    for (int i = 0; i < upcomingEvents.Count; i++)
                    {
                        Event ev = upcomingEvents[i];
                        Console.WriteLine($"{i + 1}. {ev.Name} - {ev.Date.ToShortDateString()} at {ev.Location}");
                    }
                    Console.WriteLine("==================================");

                    //Allow the admin to select an event on the list
                    Console.WriteLine("\nEnter the number of the event you want to cancel:\n(0: Back) ");
                    int selectedIndex = ExceptionHandling.IntHandling();
                    if (selectedIndex > 0 && selectedIndex <= upcomingEvents.Count)
                    {
                        Event selectedEvent = upcomingEvents[selectedIndex - 1];

                        //Confirming the cancellation
                        Console.Write($"Are you sure you want to cancel the event '{selectedEvent.Name}'? (y/n): ");
                        string confirmation = ExceptionHandling.StringHandling();

                        if (confirmation.ToLower() == "y")
                        {
                            // Step 5: Cancel the event by updating the status to 'cancelled' in the database
                            using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                            {
                                connection.Open();

                                string cancelQuery = "UPDATE dbo.[event] SET status = 'cancelled' WHERE eventID = @EventID";
                                using (SqlCommand command = new SqlCommand(cancelQuery, connection))
                                {
                                    command.Parameters.AddWithValue("@EventID", selectedEvent.EventId);

                                    int rowsAffected = command.ExecuteNonQuery();

                                    if (rowsAffected > 0)
                                    {
                                        Console.WriteLine("Event cancelled successfully.");
                                        OnEventCancelled(EventArgs.Empty);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Failed to cancel the event.");
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Event cancellation aborted.");
                        }
                    }
                    else if(selectedIndex == 0)
                    {
                        DisplayMenu();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid selection.");
                    }

                    ;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("An Error Occured while cancelling event: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An Error Occured: " + ex.Message);
            }
            finally
            {
                BackToMainMenu();
            }
        }

        public void OnEventCancelled(EventArgs e)
        {
            EventCancelled?.Invoke(null, e);
        }

        //Allows an admin to register a new organizer. Select from an existing user or create a new account to become an organizer
        public void RegisterNewOrganizer()
        {
            Console.Clear();
            Console.WriteLine("Register New Organizer\n==================================");
            Console.WriteLine("\n1. Create New User\n2. Select Existing User\n3. Cancel");
            switch (ExceptionHandling.IntHandling())
            {
                case 1:
                    try
                    {
                        Console.Write("Enter username: ");
                        string username = ExceptionHandling.StringHandling();
                        Console.Write("Enter password: ");
                        string password = Register_Login.ReadPasswordFromConsole();
                        Console.WriteLine();
                        using (SqlConnection connection = new SqlConnection(eventManager.ConnectionString))
                        {
                            connection.Open();

                            // Insert the new user into the User table and get the new UserID
                            string insertUserQuery = @"
                INSERT INTO dbo.[user](username, [Password]) 
                VALUES (@username, @password); 
                SELECT CAST(scope_identity() AS int);";

                            SqlCommand command = new SqlCommand(insertUserQuery, connection);
                            command.Parameters.AddWithValue("@username", username);
                            command.Parameters.AddWithValue("@password", password);

                            // Execute the command and retrieve the UserID of the newly inserted user
                            int newUserID = (int)command.ExecuteScalar();

                            if (newUserID > 0)
                            {
                                // Insert the UserID into the Organizer table
                                SqlCommand organizerCommand = new SqlCommand("INSERT INTO Organizer (UserID) VALUES (@UserID)", connection);
                                organizerCommand.Parameters.AddWithValue("@UserID", newUserID);

                                int rowsAffected = organizerCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    Console.WriteLine("User registered and added to Organizer successfully!");
                                }
                                else
                                {
                                    Console.WriteLine("User registered, but failed to add to Organizer.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Failed to register user. Press any key to go back...");
                            }

                            Console.ReadKey();
                            DisplayMenu();
                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("Error registering user: " + ex.Message);
                        Console.ReadKey();
                        DisplayMenu();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                        Console.ReadKey();
                        DisplayMenu();
                    }
                    break;
                case 2:
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(eventManager.ConnectionString))
                        {
                            connection.Open();

                            // Retrieve and display a list of existing users
                            string selectUsersQuery = "SELECT userID, username  FROM dbo.[user] Order BY userID";
                            SqlCommand selectCommand = new SqlCommand(selectUsersQuery, connection);

                            using (SqlDataReader reader = selectCommand.ExecuteReader())
                            {
                                Console.WriteLine("Existing Users:");
                                while (reader.Read())
                                {
                                    Console.WriteLine($"UserID: {reader["userID"]}, Username: {reader["username"]}");
                                }
                            }

                            Console.Write("Enter the UserID of the user to add as an Organizer: ");
                            int userID = ExceptionHandling.IntHandling();

                            AddUserToOrganizer(connection, userID);
                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("Error selecting user: " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }

                    BackToMainMenu();
                    break;
                case 3:
                    {
                        DisplayMenu();
                        break;
                    }
            }
        }

        //Method to display all users by type
        public void DisplayAllUsers()
        {
            Console.Clear();
            List<string> admins = new List<string>();
            List<string> organizers = new List<string>();
            List<string> regularUsers = new List<string>();

            try
            {
                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                {
                    connection.Open();

                    // Query to get all admins
                    string adminQuery = @"
                SELECT U.username 
                FROM dbo.[user] U
                INNER JOIN dbo.admin A ON U.userID = A.userID";

                    SqlCommand adminCommand = new SqlCommand(adminQuery, connection);
                    using (SqlDataReader reader = adminCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            admins.Add(reader["username"].ToString());
                        }
                    }

                    // Query to get all organizers
                    string organizerQuery = @"
                SELECT U.username 
                FROM dbo.[user] U
                INNER JOIN dbo.organizer O ON U.userID = O.userID";

                    SqlCommand organizerCommand = new SqlCommand(organizerQuery, connection);
                    using (SqlDataReader reader = organizerCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            organizers.Add(reader["username"].ToString());
                        }
                    }

                    // Query to get all users who are not in the admin or organizer tables
                    string regularUserQuery = @"
                SELECT U.username 
                FROM dbo.[user] U
                LEFT JOIN dbo.admin A ON U.userID = A.userID
                LEFT JOIN dbo.organizer O ON U.userID = O.userID
                WHERE A.userID IS NULL AND O.userID IS NULL";

                    SqlCommand regularUserCommand = new SqlCommand(regularUserQuery, connection);
                    using (SqlDataReader reader = regularUserCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            regularUsers.Add(reader["username"].ToString());
                        }
                    }
                }

                // Displaying the results
                Console.WriteLine("------- Admins -------");
                foreach (var admin in admins)
                {
                    Console.WriteLine(admin);
                }

                Console.WriteLine("\n------- Organizers -------");
                foreach (var organizer in organizers)
                {
                    Console.WriteLine(organizer);
                }

                Console.WriteLine("\n------- Regular Users -------");
                foreach (var user in regularUsers)
                {
                    Console.WriteLine(user);
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("An error occurred while retrieving users: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
            }
            finally
            {
                BackToMainMenu();
            }
        }

        
        //Helper method for adding a user to the organizer table
        private void AddUserToOrganizer(SqlConnection connection, int userID)
        {
            // Check if the UserID already exists in the Organizer table
            string checkUserQuery = "SELECT COUNT(*) FROM organizer WHERE userID = @UserID";
            SqlCommand checkCommand = new SqlCommand(checkUserQuery, connection);
            checkCommand.Parameters.AddWithValue("@UserID", userID);

            int userExists = (int)checkCommand.ExecuteScalar();

            if (userExists > 0)
            {
                Console.WriteLine("This user is already an Organizer.");
            }
            else
            {
                // Insert the UserID into the Organizer table if it does not exist
                SqlCommand organizerCommand = new SqlCommand("INSERT INTO organizer (UserID) VALUES (@UserID)", connection);
                organizerCommand.Parameters.AddWithValue("@UserID", userID);

                int rowsAffected = organizerCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("User added to Organizer successfully!");
                }
                else
                {
                    Console.WriteLine("Failed to add user to Organizer.");
                }
            }
        }

        //View the feedback of all events including the most recent comments and the average rating
        public void ViewFeedback()
        {
            Console.Clear();

            try
            {
                using (SqlConnection connection = new SqlConnection(eventManager.ConnectionString))
                {
                    connection.Open();

                    // Query to get event names, average rating, and latest three comments
                    string feedbackQuery = @"
                SELECT 
                    e.eventID, 
                    e.name, 
                    CAST(AVG(CAST(f.rating AS DECIMAL(3,2))) AS FLOAT) AS averageRating,
                    STRING_AGG(f.comment, '|') WITHIN GROUP (ORDER BY f.comment DESC) AS latestComments
                FROM 
                    feedback f
                INNER JOIN 
                    event e ON f.eventID = e.eventID
                GROUP BY 
                    e.eventID, e.name";

                    SqlCommand command = new SqlCommand(feedbackQuery, connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<int> eventIDs = new List<int>(); // List to keep track of event IDs

                        Console.WriteLine("Event Feedback Summary:");
                        int eventNumber = 1;

                        while (reader.Read())
                        {
                            Console.WriteLine("==================================");
                            int eventID = (int)reader["eventID"];
                            string eventName = reader["name"].ToString();

                            // Safely retrieving the average rating
                            double averageRating = reader["averageRating"] != DBNull.Value ? (double)reader["averageRating"] : 0;
                            string[] comments = reader["latestComments"]?.ToString().Split('|') ?? new string[0];

                            // Add event ID to list
                            eventIDs.Add(eventID);

                            Console.WriteLine($"\n{eventNumber}. Event: {eventName}");
                            Console.WriteLine($"   Average Rating: {averageRating:F2}");
                            Console.WriteLine("   Latest Comments:");

                            //Displaying comments
                            for (int i = 0; i < Math.Min(comments.Length, 3); i++)
                            {
                                Console.WriteLine($"   - {comments[i]}");
                            }

                            eventNumber++;
                        }

                        reader.Close();

                        // Prompt the admin to select an event based on its number
                        Console.WriteLine("\nSelect the event number to view all comments:\n (0: Back) ");
                        int selectedEventNumber = ExceptionHandling.IntHandling();

                        if (selectedEventNumber > 0 && selectedEventNumber <= eventIDs.Count)
                        {
                            int selectedEventID = eventIDs[selectedEventNumber - 1];
                            ViewAllCommentsForEvent(connection, selectedEventID);
                        }
                        else if(selectedEventNumber ==0)
                        {
                            
                        }
                        else
                        {
                            Console.WriteLine("Invalid selection.");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error retrieving feedback: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                BackToMainMenu();
            }
        }

        //Helper Method for viewing the feedback.
        //This method will get all the comments for a selected event
        private void ViewAllCommentsForEvent(SqlConnection connection, int eventID)
        {
            Console.Clear();

            try
            {
                // Query to get all comments for the selected event
                string commentsQuery = @"
            SELECT 
                comment 
            FROM 
                feedback 
            WHERE 
                eventID = @eventID 
            ORDER BY 
                comment DESC";

                SqlCommand command = new SqlCommand(commentsQuery, connection);
                command.Parameters.AddWithValue("@eventID", eventID);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine($"\nAll Comments for EventID: {eventID}");
                    while (reader.Read())
                    {
                        string comment = reader["comment"].ToString();
                        Console.WriteLine($"- {comment}");
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error retrieving comments: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            Console.ReadKey();
            BackToMainMenu();
        }

        


        //Calls the Register_Login class to log out the user
        public override void Logout()
        {
            try
            {
                Console.WriteLine("\nAdmin logout successful!");
                Thread.Sleep(1500);
                Register_Login.DisplayMenu();
                

            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error logging out: " + ex);
                BackToMainMenu();
            }
        }
        //Helper method to element the repetiveness of returning to the menu
        public void BackToMainMenu()
        {
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
            Console.Clear();
            DisplayMenu();
        }

    }
}
