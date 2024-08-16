using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Caydan Frank
            //Hello people
            //This is a comment
            //markus
            Console.WriteLine("Hello Good mense"); // I am chaning this Joseph
            //Theuns comment
            //gooner alert
            //gooner alert2
            //Here is another change for you grandpa
            //hallo

            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "myfreesqldbserverkys.database.windows.net";
                builder.UserID = "prg281EventManagementDB";
                builder.Password = "1Vjs.B_%X$4sPjX0";
                builder.InitialCatalog = "Event_Management";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    String sql = "select * from Test";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("{0}", reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadLine();
        }
    }
}
