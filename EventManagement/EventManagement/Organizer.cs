﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventManagement
{
    internal class Organizer : User, IEventManager, IOrganizer
    {
        public Organizer(int id, string userName, string password) : base(id, userName, password)
        {

        }

        public enum OrganizerMenuOptions
        {
            View_Upcoming_Events = 1,
            Create_New_Event,
            Past_Feedback,
            Edit_Details,
            Log_out
        }

        //Displaying menu for organizer
        public override void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("Organizer Menu:");

            foreach (OrganizerMenuOptions option in Enum.GetValues(typeof(OrganizerMenuOptions)))
            {
                string optionName = option.ToString().Replace("_", " ");
                Console.WriteLine($"{(int)option}. {optionName}");
            }

            Console.Write("Select an option (1-5): ");
            string input = ExceptionHandling.StringHandling();


            if (Enum.TryParse(input, out OrganizerMenuOptions chosenOption) && Enum.IsDefined(typeof(OrganizerMenuOptions), chosenOption))
            {
                switch (chosenOption)
                {
                    case OrganizerMenuOptions.View_Upcoming_Events:
                        DisplayOrganizedEvents();
                        Console.WriteLine();
                        Console.WriteLine("==============================================");
                        DisplayMenu();
                        break;
                    case OrganizerMenuOptions.Create_New_Event:
                        CreateEvent();
                        Console.WriteLine();
                        Console.WriteLine("==============================================");
                        DisplayMenu();
                        break;
                    case OrganizerMenuOptions.Past_Feedback:
                        ViewFeedback();
                        Console.WriteLine();
                        Console.WriteLine("==============================================");
                        DisplayMenu();
                        break;
                    case OrganizerMenuOptions.Edit_Details:
                        Register_Login.currentUser.ManageProfile();
                        Thread.Sleep(500);
                        DisplayMenu();
                        break;
                    case OrganizerMenuOptions.Log_out:
                        Logout();
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Invalid Selection. Try again:");
                        DisplayMenu();
                        break;
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Invalid option, please try again.");
                DisplayMenu();
            }


        }

        public override void Logout()
        {
            try
            {

                Console.WriteLine("Organizer logout successful!");
                Thread.Sleep(1000);
                Register_Login.DisplayMenu();

            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error logging out: " + ex);
                BackToMainMenu();
            }
        }

        public static void BackToMainMenu()
        {
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
            Console.Clear();
        }

        //Display all events associated with the logged in organizer
        public void DisplayOrganizedEvents()
        {
            Console.Clear();
            List<Event> events = GetEvents();

            Console.WriteLine("Upcoming Events which you are organising:");
            for (int i = 0; i < events.Count; i++)
            {
                //Only displaying events that have not ended
                if (events[i].Status == "upcoming" || events[i].Status == "pending" || events[i].Status == "postponed")
                {
                    Console.WriteLine($"{i + 1}. {events[i].Name} - {events[i].Location} - {events[i].Date.ToShortDateString()} - {events[i].Status}");
                }
            }

            //Getting event that the organizer selected
            Console.WriteLine("\nSelect an event (enter number) or type 'back' to go back:");
            string input = ExceptionHandling.StringHandling();

            if (input.ToLower() == "back")
            {
                Console.Clear();
                return;
            }

            if (int.TryParse(input, out int choice) && choice > 0 && choice <= events.Count)
            {
                DisplayEventDetails(events[choice - 1]);
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }
        }

        //Getting a List that contains all the events for the organizer
        public List<Event> GetEvents()
        {
            List<Event> events = new List<Event>();
            using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM event WHERE organizerID= (SELECT organizerID FROM organizer where userID=@userID) ORDER BY date ASC"; //SQL Query to get the specified table 

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@userID", this.id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Event ev = new Event()
                            {
                                EventId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                Date = reader.GetDateTime(3),
                                Location = reader.GetString(4),
                                OrganizerId = reader.GetInt32(5),
                                Status = reader.GetString(6),
                                TicketPrice = Math.Round(reader.GetDecimal(7), 2)
                            };
                            events.Add(ev);
                        }
                    }
                }
            }
            return events;
        }

        //Displaying all the details of a specific even
        public void DisplayEventDetails(Event ev)
        {
            Console.WriteLine($"\nEvent Details\n==================================");
            Console.WriteLine($"Name: {ev.Name}");
            Console.WriteLine($"Description: {ev.Description}");
            Console.WriteLine($"Date: {ev.Date.ToShortDateString()}");
            Console.WriteLine($"Location: {ev.Location}");
            Console.WriteLine($"Organizer ID: {ev.OrganizerId}"); // Don't think it is necessary to show this as it's already filtered
            Console.WriteLine($"Status: {ev.Status}");
            Console.WriteLine($"Ticket Price: {ev.TicketPrice}");

            MenuEditEvent(ev);
        }

        //Validating user input to enter an appropriate date
        public DateTime CheckDate()
        {
            DateTime eventDate = ExceptionHandling.DateHandling();
            if (eventDate > DateTime.Now)
            {
                return eventDate;
            }
            else
            {
                Console.WriteLine("You cannot enter {0}", eventDate == DateTime.Now ? "todays date (must be later)" : "a date which has passed");
                Console.WriteLine("1. Enter a later date\n2. Go back to menu");
                int option = ExceptionHandling.IntHandling();
                switch (option)
                {
                    case 1:
                        Console.WriteLine("Enter the later date:");
                        return CheckDate();
                        break;
                    case 2:
                        DisplayMenu();
                        break;
                    default:
                        Console.WriteLine("You have entered an invalid option");
                        Thread.Sleep(1000);
                        Console.WriteLine("Returning to menu");
                        Thread.Sleep(1000);
                        Console.Write(".");
                        Thread.Sleep(1000);
                        Console.Write(".");
                        Thread.Sleep(1000);
                        Console.Write(".");
                        Thread.Sleep(1000);
                        DisplayMenu();
                        break;
                }

                return eventDate;
            }
        }

        //Creating an event
        public void CreateEvent()
        {
            Console.Clear();
            Console.Write("Enter name of event: ");
            string eventName = ExceptionHandling.StringHandling();
            Console.Write("Enter description of event: ");
            string eventDescription = ExceptionHandling.StringHandling();
            Console.Write("Enter date of event: ");
            DateTime eventDate = CheckDate();
            Console.Write("Enter location of event: ");
            string eventLocation = ExceptionHandling.StringHandling();
            string eventStatus = "pending"; // Event status set to 'pending' by default
            Console.Write("Enter ticket price: ");
            double ticketPrice = ExceptionHandling.DoubleHandling();
            Console.WriteLine();

            try
            {
                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                {
                    connection.Open();

                    // Step 1: Retrieve the organizerID from the Organizer table based on the current user's ID
                    string getOrganizerIdQuery = "SELECT organizerID FROM dbo.Organizer WHERE userID = @UserID";
                    SqlCommand getOrganizerIdCommand = new SqlCommand(getOrganizerIdQuery, connection);
                    getOrganizerIdCommand.Parameters.AddWithValue("@UserID", Register_Login.currentUser.ID);

                    object organizerIdObj = getOrganizerIdCommand.ExecuteScalar();

                    if (organizerIdObj == null)
                    {
                        Console.WriteLine("You are not registered as an organizer.");
                        Console.ReadKey();
                        BackToMainMenu();
                        return;
                    }

                    int organizerID = Convert.ToInt32(organizerIdObj);

                    // Step 2: Insert the new event into the Event table
                    SqlCommand command = new SqlCommand("INSERT INTO dbo.[event](name, description, date, location, organizerID, status, ticket_price) VALUES (@name, @description, @date, @location, @organizerID, @status, @ticket_price)", connection);
                    command.Parameters.AddWithValue("@name", eventName);
                    command.Parameters.AddWithValue("@description", eventDescription);
                    command.Parameters.AddWithValue("@date", eventDate);
                    command.Parameters.AddWithValue("@location", eventLocation);
                    command.Parameters.AddWithValue("@organizerID", organizerID);
                    command.Parameters.AddWithValue("@status", eventStatus);
                    command.Parameters.AddWithValue("@ticket_price", ticketPrice);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Event created successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Failed to create event.");
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: " + ex.Message);
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



        //Viewing feedback for past event that the organizer created
        public void ViewFeedback()
        {
            Console.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
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
                WHERE organizerID= (SELECT organizerID FROM organizer where userID=@userID)	AND e.status = 'ended'
                GROUP BY 
                    e.eventID, e.name";

                    SqlCommand command = new SqlCommand(feedbackQuery, connection);
                    command.Parameters.AddWithValue("@userID", this.id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<int> eventIDs = new List<int>(); // List to keep track of event IDs

                        Console.WriteLine("Event Feedback Summary:");
                        int eventNumber = 1;

                        while (reader.Read())
                        {
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

                            for (int i = 0; i < Math.Min(comments.Length, 3); i++)
                            {
                                Console.WriteLine($"   - {comments[i]}");
                            }

                            eventNumber++;
                        }
                        reader.Close();

                        // Prompt the admin to select an event based on its number
                        Console.Write("\nSelect the event number to view all comments: ");
                        int selectedEventNumber = ExceptionHandling.IntHandling();

                        if (selectedEventNumber > 0 && selectedEventNumber <= eventIDs.Count)
                        {
                            int selectedEventID = eventIDs[selectedEventNumber - 1];
                            ViewAllCommentsForEvent(connection, selectedEventID);
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
                
            }
        }


        //Viewing all comments for a past event 
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
            BackToMainMenu();
       

        }

        //Menu that displays when an organizer selects the Edit Event option
        private void MenuEditEvent(Event ev)
        {
            Console.WriteLine();
            Console.WriteLine("1. Edit the event");
            Console.WriteLine("2. Go back to menu");
            int input = ExceptionHandling.IntHandling();
            switch (input)
            {
                case 1:
                    //Making sure organizer cannot edit an event that has already passed
                    if (ev.Status == "pending" || ev.Status == "upcoming" || ev.Status == "postponed")
                    {
                        EditEvent(ev);
                    }
                    else
                    {
                        Console.WriteLine("Sorry you cannot edit this event as it {0}", ev.Status == "cancelled" ? "has been cancelled." : "has ended.");
                        Thread.Sleep(1000);
                        Console.WriteLine("Sending you back to the menu");
                        Thread.Sleep(1000);
                        Console.Write(".");
                        Thread.Sleep(1000);
                        Console.Write(".");
                        Thread.Sleep(1000);
                        Console.Write(".");
                        Thread.Sleep(1000);
                        DisplayMenu();
                    }

                    break;
                case 2:
                    DisplayMenu();
                    break;
                default:
                    Console.WriteLine("Something went wrong");
                    Console.WriteLine("Please enter a valid menu option");
                    MenuEditEvent(ev);
                    break;
            }

        }



        //Editing the details of a specific event
        public void EditEvent(Event ev)
        {
            Console.Clear();
            Console.WriteLine("Edit Event\n==============================");
            Console.WriteLine();
            //Getting the field that needs to be updated
            Console.WriteLine("What do you want to edit");
            Console.WriteLine("1. Event Name\n2. Event Description\n3. Event Date\n4. Event location\n5. Event Ticket Price\n6. Back");
            int option = ExceptionHandling.IntHandling();
            switch (option)
            {
                case 1:
                    //Updating the name of the event
                    Console.WriteLine("Previous name: {0}", ev.Name);
                    Console.WriteLine("Enter the new Event Name.");
                    string name = ExceptionHandling.StringHandling();

                    try
                    {
                        using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                        {
                            connection.Open();

                            SqlCommand command = new SqlCommand("UPDATE [event] SET name = @name WHERE eventID = @eventID", connection);
                            command.Parameters.AddWithValue("@eventID", ev.EventId);
                            command.Parameters.AddWithValue("@name", name);




                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("Event updated successfully!");

                            }
                            else
                            {
                                Console.WriteLine("Failed to update event. Please try again.");
                            }

                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("An error occurred while updating your event: " + ex.Message);
                    }

                    break;
                case 2:
                    //Updating the description of the event
                    Console.WriteLine("Previous description: {0}", ev.Description);
                    Console.WriteLine("Enter the new Event Description.");
                    string description = ExceptionHandling.StringHandling();

                    try
                    {
                        using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                        {
                            connection.Open();

                            SqlCommand command = new SqlCommand("UPDATE [event] SET description = @description WHERE eventID = @eventID", connection);
                            command.Parameters.AddWithValue("@eventID", ev.EventId);
                            command.Parameters.AddWithValue("@description", description);


                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("Event updated successfully!");

                            }
                            else
                            {
                                Console.WriteLine("Failed to update event. Please try again.");
                            }

                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("An error occurred while updating your event: " + ex.Message);
                    }
                    break;
                case 3:
                    //Updating the date of the event
                    Console.WriteLine("Previous event date: {0}", ev.Date);
                    Console.WriteLine("Enter the new Event Date.");
                    DateTime date = CheckDate();

                    try
                    {
                        using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                        {
                            connection.Open();

                            SqlCommand command = new SqlCommand("UPDATE [event] SET date = @date WHERE eventID = @eventID", connection);
                            command.Parameters.AddWithValue("@eventID", ev.EventId);
                            command.Parameters.AddWithValue("@date", date);




                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("Event updated successfully!");

                            }
                            else
                            {
                                Console.WriteLine("Failed to update event. Please try again.");
                            }

                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("An error occurred while updating your event: " + ex.Message);
                    }
                    break;
                case 4:
                    //Updating the location of the event
                    Console.WriteLine("Previous location: {0}", ev.Location);
                    Console.WriteLine("Enter the new Event Location.");
                    string location = ExceptionHandling.StringHandling();

                    try
                    {
                        using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                        {
                            connection.Open();

                            SqlCommand command = new SqlCommand("UPDATE [event] SET location = @location WHERE eventID = @eventID", connection);
                            command.Parameters.AddWithValue("@eventID", ev.EventId);
                            command.Parameters.AddWithValue("@location", location);




                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("Event updated successfully!");

                            }
                            else
                            {
                                Console.WriteLine("Failed to update event. Please try again.");
                            }

                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("An error occurred while updating your event: " + ex.Message);
                    }
                    break;
                case 5:
                    //Updating the ticket price of an event
                    Console.WriteLine("Previous ticket price: {0}", ev.TicketPrice);
                    Console.WriteLine("Enter the new Event Ticket Price.");
                    double ticketPrice = ExceptionHandling.DoubleHandling();

                    try
                    {
                        using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                        {
                            connection.Open();

                            SqlCommand command = new SqlCommand("UPDATE [event] SET ticket_price = @ticket_price WHERE eventID = @eventID", connection);
                            command.Parameters.AddWithValue("@eventID", ev.EventId);
                            command.Parameters.AddWithValue("@ticket_price", ticketPrice);




                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("Event updated successfully!");

                            }
                            else
                            {
                                Console.WriteLine("Failed to update event. Please try again.");
                            }

                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("An error occurred while updating your event: " + ex.Message);
                    }
                    break;
                case 6:
                    //Sending organizer back to previous menu
                    MenuEditEvent(ev);
                    break;
                default:
                    //Displaying error and sending organizer back to previous menu if invalid option is selected
                    Console.WriteLine("Something went wrong :(");
                    Thread.Sleep(1000);
                    Console.WriteLine("Sending you back to the previous menu");
                    Thread.Sleep(1000);
                    Console.Write(".");
                    Thread.Sleep(1000);
                    Console.Write(".");
                    Thread.Sleep(1000);
                    Console.Write(".");
                    Thread.Sleep(1000);
                    MenuEditEvent(ev);
                    break;
            }
            Console.Clear();
            MenuEditEvent(ev);


        }




    }


}
