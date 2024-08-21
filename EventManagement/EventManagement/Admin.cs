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

                            break;
                        case AdminMenuOptions.Create_New_Event:


                            break;
                        case AdminMenuOptions.Register_New_Organizer:

                            break;
                        case AdminMenuOptions.Past_Feedback:

                            break;
                        case AdminMenuOptions.Edit_Details:

                            break;
                        case AdminMenuOptions.Log_out:

                            break;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid option, please try again.");
                }
            
        }

        public override void SignUp()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            // Check if username already exists
            using (SqlConnection connection = new SqlConnection("Data Source=<database_server>;Initial Catalog=<database_name>;User ID=<username>;Password=<password>"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM Admins WHERE Username = @username", connection);
                command.Parameters.AddWithValue("@username", username);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    Console.WriteLine("Username already exists. Please choose a different username.");
                    return;
                }
            }

            // Add user to database
            using (SqlConnection connection = new SqlConnection("Data Source=<database_server>;Initial Catalog=<database_name>;User ID=<username>;Password=<password>"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("INSERT INTO Admins (Username, Password) VALUES (@username, @password)", connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                command.ExecuteNonQuery();
            }

            Console.WriteLine("Admin sign up successful!");
        }

        public override void Login()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection("Data Source=<database_server>;Initial Catalog=<database_name>;User ID=<username>;Password=<password>"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM Admins WHERE Username = @username AND Password = @password", connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    Console.WriteLine("Admin login successful!");
                    DisplayMenu();
                }
                else
                {
                    Console.WriteLine("Invalid username or password.");
                }
            }

        }
        public override void Logout()
        {
            Console.WriteLine("Admin logout successful!");
        }

        public static void BackToMainMenu()
        {
            Console.WriteLine("Press any key to return to the admin menu...");
            Console.ReadKey();
            Console.Clear();
        }

    }
}
