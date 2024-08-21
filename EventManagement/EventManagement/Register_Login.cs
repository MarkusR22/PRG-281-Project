using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement
{
    internal class Register_Login
    {

        static void DisplayMenu()
        {
            Console.WriteLine("Event Management System");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.Write("Choose an option: ");
            int option = Convert.ToInt32(Console.ReadLine());

            switch (option)
            {
                case 1:
                    RegisterUser();
                    break;
                case 2:
                    LoginUser();
                    break;
                default:
                    Console.WriteLine("Invalid option. Exiting...");
                    break;
            }
        }
        static void RegisterUser()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = ReadPasswordFromConsole();

            try
            {



                using (SqlConnection connection = new SqlConnection("Data Source=MACHINE;Initial Catalog=EventManagementTemp;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("INSERT INTO dbo.[user](username, [Password]) VALUES (@username, @password)", connection);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    int rowsAffected = (int)command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("User registered successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Failed to register user.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void LoginUser()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = ReadPasswordFromConsole();

            using (SqlConnection connection = new SqlConnection("Data Source=MACHINE;Initial Catalog=EventManagementTemp;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM [user] WHERE username = @username AND [Password] = @password", connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Console.WriteLine("Login successful!");
                    // You can add additional logic here to authenticate the user
                }
                else
                {
                    Console.WriteLine("Invalid username or password.");
                }
            }
        }

        static string ReadPasswordFromConsole()
        {
            string password = "";
            ConsoleKeyInfo info;

            do
            {
                info = Console.ReadKey(true);

                if (info.Key != ConsoleKey.Enter)
                {
                    if (info.Key == ConsoleKey.Backspace)
                    {
                        if (password.Length > 0)
                        {
                            Console.Write("\b \b"); // Backspace and overwrite with space
                            password = password.Substring(0, password.Length - 1);
                        }
                    }
                    else
                    {
                        Console.Write("*");
                        password += info.KeyChar;
                    }
                }
            }
            while (info.Key != ConsoleKey.Enter);

            return password;
        }
    }
}


}
