using EventManagement;
using System.Data.SqlClient;
using System;
using System.Runtime.InteropServices;

internal class Register_Login
{

    public static User currentUser;
    

    public static User CurrentUser
    {
        get { return currentUser; }
    }

    
    //private static string connectionString = "Data Source=MACHINE;Initial Catalog=EventManagementTemp;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"; // Caydan
    //private static string connectionString = "Data Source=TIMOTHY\\MSSQLSERVER09;Initial Catalog=EventManagementTemp;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"; //Joseph's DB connection
    //private static string connectionString = "Data Source=DESKTOP-TDBJOM7;Initial Catalog=EventManagement;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"; //Markus' connection string
    //private static string connectionString = "Data Source=EE-GAMINGPC;Initial Catalog=EventManagementTheuns;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"; //Theuns db string


    //Login And Register Meu
    public static void DisplayMenu()
    {
        currentUser = null;

        Console.Clear();

        Console.WriteLine("Event Management System");
        Console.WriteLine("1. Register");
        Console.WriteLine("2. Login");
        Console.Write("Choose an option: ");
        //try
        //{
            int option = ExceptionHandling.IntHandling();
            switch (option)
            {
                case 1:
                    RegisterUser();
                    break;
                case 2:
                    LoginUser();
                    break;
                default:
                    Console.WriteLine("Invalid option. Press any button to go back");
                    DisplayMenu();
                    break;
            }
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine("Invalid option. Press any button to go back");
        //    Console.ReadKey();
        //    DisplayMenu();

        //}
    }

    static void RegisterUser()
    {
        Console.Write("Enter username: ");
        string username = Console.ReadLine();
        Console.Write("Enter password: ");
        string password = ReadPasswordFromConsole();
        Console.WriteLine();

        try
        {
            using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("INSERT INTO dbo.[user](username, [Password]) VALUES (@username, @password)", connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                int rowsAffected = (int)command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("User registered successfully!");
                    Console.ReadKey();
                    DisplayMenu();
                }
                else
                {
                    Console.WriteLine("Failed to register user. Press any key to go back...");
                    Console.ReadKey();
                    DisplayMenu();
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Error registering user: Username taken");
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

    static void LoginUser()
    {
        Console.Write("Enter username: ");
        string username = Console.ReadLine();
        Console.Write("Enter password: ");
        string password = ReadPasswordFromConsole();
        Console.WriteLine();

        try
        {
            using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM [user] WHERE username = @username AND [Password] = @password", connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int userId = (int)reader["userID"];
                    User user = GetUserType(userId);

                    if (user != null)
                    {
                        Console.WriteLine("Login successful!");
                        DisplayMenuBasedOnUserType(user);
                    }
                    else
                    {
                        Console.WriteLine("Failed to determine user type.");
                        Console.ReadKey();
                        DisplayMenu();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid username or password. Press any key to go back...");
                    Console.ReadKey();
                    DisplayMenu();
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Error logging in: " + ex.Message);
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

    public static string ReadPasswordFromConsole()
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

    static User GetUserType(int userId)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
            {
                connection.Open();

                SqlCommand userCommand = new SqlCommand("SELECT username, password FROM [user] WHERE userID = @userId", connection);
                userCommand.Parameters.AddWithValue("@userId", userId);

                SqlDataReader userReader = userCommand.ExecuteReader();

                if (userReader.Read())
                {
                    string userName = (string)userReader["username"];
                    string password = (string)userReader["password"];

                    userReader.Close();

                    SqlCommand adminCommand = new SqlCommand("SELECT * FROM admin WHERE userID = @userId", connection);
                    adminCommand.Parameters.AddWithValue("@userId", userId);

                    SqlDataReader adminReader = adminCommand.ExecuteReader();

                    if (adminReader.Read())
                    {
                        currentUser = new Admin(userId, userName, password);
                        return currentUser;
                    }

                    adminReader.Close();

                    SqlCommand organizerCommand = new SqlCommand("SELECT * FROM organizer WHERE userID = @userId", connection);
                    organizerCommand.Parameters.AddWithValue("@userId", userId);

                    SqlDataReader organizerReader = organizerCommand.ExecuteReader();

                    if (organizerReader.Read())
                    {
                        currentUser = new Organizer(userId, userName, password);
                        return currentUser;
                    }
                    currentUser = new Participant(userId, userName, password);
                    return currentUser;
                }

                return null; // or throw an exception if user is not found
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Error determining user type: " + ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return null;
        }
    }


    static void DisplayMenuBasedOnUserType(User user)
    {
        currentUser.DisplayMenu();
    }
}