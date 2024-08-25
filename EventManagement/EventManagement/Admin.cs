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
                        View_Upcoming_Events();

                        break;
                    case AdminMenuOptions.Create_New_Event:
                        CreateEvent();

                        break;
                    case AdminMenuOptions.Register_New_Organizer:

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
            EventManager eventManager = new EventManager(connectionString);

            eventManager.DisplayUpcommingEvents();
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

        public override void Logout()
        {
            Console.WriteLine("Admin logout successful! Press any key to open Login Menu");
            Register_Login.DisplayMenu();
        }

        public static void BackToMainMenu()
        {
            Console.WriteLine("Press any key to return to the admin menu...");
            Console.ReadKey();
            Console.Clear();
        }

    }
}
