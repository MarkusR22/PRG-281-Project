using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement
{

    public class Admin : User
    {
        EventManager eventManager = new EventManager(connectionString);
        private static string connectionString = "Data Source=MACHINE;Initial Catalog=EventManagementTemp;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"; // Caydan
        //private static string connectionString = "Data Source=TIMOTHY\\MSSQLSERVER09;Initial Catalog=EventManagementTemp;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"; //Joseph's DB connection
        //private static string connectionString = "Data Source=DESKTOP-TDBJOM7;Initial Catalog=EventManagement;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"; //Markus' connection string
        //private static string connectionString = "Data Source=EE-GAMINGPC;Initial Catalog=EventManagementTheuns;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"; //Theuns db string


        public Admin(int id, string userName, string password) : base(id, userName, password)
        {

        }

        public enum AdminMenuOptions
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
            Console.Clear();
            Console.WriteLine($"Welcome Admin {userName}:");

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
                        View_Upcoming_Events();

                        break;
                    case AdminMenuOptions.Create_New_Event:
                        CreateEvent();

                        break;
                    case AdminMenuOptions.Register_New_Organizer:
                        RegisterNewOrganizer();

                        break;
                    case AdminMenuOptions.Past_Feedback:

                        break;
                    case AdminMenuOptions.Edit_Details:
                        break;
                    case AdminMenuOptions.Log_out:
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

        public void View_Upcoming_Events()
        {
            eventManager.DisplayUpcommingEvents();
        }

        public void RegisterNewOrganizer()
        {
            Console.WriteLine("\n1. Create New User\n2. Select Existing User");
            switch (int.Parse(Console.ReadLine()))
            {
                case 1:
                    try
                    {
                        Console.Write("Enter username: ");
                        string username = Console.ReadLine();
                        Console.Write("Enter password: ");
                        string password = Register_Login.ReadPasswordFromConsole();
                        Console.WriteLine();
                        using (SqlConnection connection = new SqlConnection(connectionString))
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
                        using (SqlConnection connection = new SqlConnection(connectionString))
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
                            int userID = int.Parse(Console.ReadLine());

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

                    Console.ReadKey();
                    DisplayMenu();
                    break;
            }
        }
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


        public void CreateEvent()
        {
            Console.Write("Enter name of event: ");
            string eventName = Console.ReadLine();
            Console.Write("Enter description of event: ");
            string eventDescription = Console.ReadLine();
            Console.Write("Enter date of event: ");
            DateTime eventDate = DateTime.Parse(Console.ReadLine());
            Console.Write("Enter location of event: ");
            string eventLocation = Console.ReadLine();
            Console.Write("Enter event organizer ID: ");
            int organizerID = int.Parse(Console.ReadLine());
            Console.Write("Enter status of event: ");
            string eventStatus = Console.ReadLine();
            Console.Write("Enter ticket price: ");
            double ticketPrice = double.Parse(Console.ReadLine());
            Console.WriteLine();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("INSERT INTO dbo.event(name, description, date, location, organizerID, status, ticket_price) VALUES (@name, @description, @date, @location, @organizerID, @status, @ticket_price)", connection);
                    command.Parameters.AddWithValue("@name", eventName);
                    command.Parameters.AddWithValue("@description", eventDescription);
                    command.Parameters.AddWithValue("@date", eventDate);
                    command.Parameters.AddWithValue("@location", eventLocation);
                    command.Parameters.AddWithValue("@organizerID", organizerID);
                    command.Parameters.AddWithValue("@status", eventStatus);
                    command.Parameters.AddWithValue("@ticket_price", ticketPrice);

                    int rowsAffected = (int)command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Event created successfully! Press any key to return to menu");
                        Console.ReadKey();
                        DisplayMenu();
                    }
                    else
                    {
                        Console.WriteLine("Failed to create event. Press any key to go back...");
                        Console.ReadKey();
                        DisplayMenu();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                DisplayMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                Console.ReadKey();
                DisplayMenu();
            }
        }

        public override async void Logout()
        {
            try
            {
                Console.WriteLine("Admin logout successful!");
                await Task.Delay(1000);
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
            Console.WriteLine("Press any key to return to the admin menu...");
            Console.ReadKey();
            Console.Clear();
        }

    }
}
