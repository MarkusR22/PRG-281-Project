using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement
{
    public class EventManager
    {
        //static User currentUser = Register_Login.CurrentUser;
        public const string connectionString = "Data Source=MACHINE;Initial Catalog=EventManagementTemp;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"; // Caydan
        //public const string connectionString = "Data Source=TIMOTHY\\MSSQLSERVER09;Initial Catalog=EventManagementTemp;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"; //Joseph's DB connection
        //public const string connectionString = "Data Source=DESKTOP-TDBJOM7;Initial Catalog=EventManagement;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"; //Markus' connection string
        public const string connectionString = "Data Source=EE-GAMINGPC;Initial Catalog=EventManagementTheuns;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"; //Theuns db string
        public string ConnectionString
        {
            get { return connectionString; }
        }

        //public EventManager(string connectionString)
        //{
        //    this.connectionString = connectionString;
        //}

        public void DisplayUpcommingEvents()
        {
            List<Event> events = GetEvents();

            Console.WriteLine("Upcoming Events:");
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].Status == "upcoming")
                {
                    Console.WriteLine($"{i + 1}. {events[i].Name} - {events[i].Location} - {events[i].Date.ToShortDateString()}");
                }
            }

            Console.WriteLine("\nSelect an event (enter number) or type 'back' to go back:");
            string input = Console.ReadLine();

            if (input.ToLower() == "back")
            {
                Register_Login.CurrentUser.DisplayMenu();
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

        public List<Event> GetEvents()
        {
            List<Event> events = new List<Event>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM event ORDER BY date ASC";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
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

        public void CreateEvent()
        {
            try
            {

                Console.Write("Enter name of event: ");
                string eventName = ExceptionHandling.StringHandling();
                Console.Write("Enter description of event: ");
                string eventDescription = ExceptionHandling.StringHandling();
                Console.Write("Enter date of event: ");
                DateTime eventDate = DateTime.Parse(Console.ReadLine());
                Console.Write("Enter location of event: ");
                string eventLocation = ExceptionHandling.StringHandling();
                Console.Write("Enter event organizer ID: ");
                int organizerID = ExceptionHandling.IntHandling();
                Console.Write("Enter status of event: ");
                string eventStatus = ExceptionHandling.StringHandling();
                Console.Write("Enter ticket price: ");
                double ticketPrice = ExceptionHandling.DoubleHandling();
                Console.WriteLine();


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
                        Console.WriteLine("Event created successfully!J");
                        Console.ReadKey();
                        Register_Login.CurrentUser.DisplayMenu();
                    }
                    else
                    {
                        Console.WriteLine("Failed to create event.");
                        Console.ReadKey();
                        Register_Login.CurrentUser.DisplayMenu();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                
            } finally
            {
                Console.WriteLine("Press any key to return to menu...");
                Console.ReadKey();
                Register_Login.CurrentUser.DisplayMenu();
            }
            
        }

        public void DisplayEventDetails(Event ev)
        {
            Console.WriteLine($"\nEvent Details:");
            Console.WriteLine($"Name: {ev.Name}");
            Console.WriteLine($"Description: {ev.Description}");
            Console.WriteLine($"Date: {ev.Date.ToShortDateString()}");
            Console.WriteLine($"Location: {ev.Location}");
            Console.WriteLine($"Organizer ID: {ev.OrganizerId}");
            Console.WriteLine($"Status: {ev.Status}");
            Console.WriteLine($"Ticket Price: {ev.TicketPrice}");
        }
    }
    public class Event
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public int OrganizerId { get; set; }
        public string Status { get; set; }
        public decimal TicketPrice { get; set; }
    }
}
