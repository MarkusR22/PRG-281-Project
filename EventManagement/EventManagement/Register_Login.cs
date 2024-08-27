using EventManagement;
using System.Data.SqlClient;
using System;
using System.Runtime.InteropServices;
using System.Threading;

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
        Console.WriteLine("3. Exit");
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
            case 3:
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid option. Press any button to go back");
                DisplayMenu();
                break;
        }
    }

    static void RegisterUser()
    {
        Console.WriteLine("==================================");
        Console.Write("Enter username: ");
        string username = ExceptionHandling.StringHandling();
        Console.Write("Enter password: ");
        string password = ReadPasswordFromConsole();
        while (password.Length < 8)
        {
            Console.WriteLine("\nPassword must contain at least 8 characters");
            Console.Write("Enter password: ");
            password = ReadPasswordFromConsole();
        }

        Console.WriteLine();
        Console.Write("Confirm Password: ");
        string confirmPassword = ReadPasswordFromConsole();
        while (!confirmPassword.Equals(password))
        {
            Console.WriteLine();
            Console.WriteLine("Passwords Do Not Match. Try Again:\n0: Cancel");
            confirmPassword = ReadPasswordFromConsole();
            if(confirmPassword == "0")
            {
                Console.WriteLine("Cancelled Registration. Press any key to return to menu");
                Console.ReadKey();
                Console.Clear();
                DisplayMenu();
                return;
            }
        }



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
                    Console.WriteLine("==================================");
                    Console.WriteLine("User registered successfully!");
                    Console.WriteLine("Logging In...");
                    Thread.Sleep(1000);

                    try
                    {

                        SqlCommand logCommand = new SqlCommand("SELECT * FROM [user] WHERE username = @username AND [Password] = @password", connection);
                        logCommand.Parameters.AddWithValue("@username", username);
                        logCommand.Parameters.AddWithValue("@password", password);

                        SqlDataReader reader = logCommand.ExecuteReader();
                        if (reader.Read())
                        {


                            int userId = (int)reader["userID"];
                            User user = GetUserType(userId);
                            Thread.Sleep(1500);
                            user.DisplayMenu();
                        }
                        else
                        {
                            Console.WriteLine("Error here");
                        }

                        reader.Close();


                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("Error logging in new user: " + ex.Message);
                        Console.ReadKey();
                        DisplayMenu();
                    }
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
            Console.WriteLine("Error registering user: " + ex.Message);
            Console.ReadKey();
            DisplayMenu();
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error registering user: " + ex.Message);
            Console.ReadKey();
            DisplayMenu();
        }
    }

    static void LoginUser()
    {
        Console.WriteLine("==================================");
        Console.Write("Enter username: ");
        string username = ExceptionHandling.StringHandling();
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
                        Console.WriteLine("==================================");
                        Console.WriteLine("Login successful!");
                        Thread.Sleep(1000);
                        user.DisplayMenu();
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
            Console.WriteLine("An error logging in: " + ex.Message);
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
            Console.WriteLine("An determining user type: " + ex.Message);
            return null;
        }
    }

}