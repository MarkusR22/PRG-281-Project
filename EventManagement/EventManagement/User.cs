using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement
{
    public abstract class User
    {
        protected int id { get; set; }
        protected string userName { get; set; }
        protected string password { get; set; }

        


        

        public abstract void DisplayMenu();
        //public abstract void Login();
        //public abstract void SignUp();
        public abstract void Logout();

        public void ManageProfile()
        {
            Console.Write("Enter new username (leave blank to keep current): ");
            string newUsername = Console.ReadLine().Trim();

            Console.Write("Enter new password (leave blank to keep current): ");
            string newPassword = Console.ReadLine().Trim();

            if (!string.IsNullOrEmpty(newUsername) || !string.IsNullOrEmpty(newPassword))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(EventManager.connectionString))
                    {
                        connection.Open();

                        SqlCommand command = new SqlCommand("UPDATE [user] SET username = @username, password = @password WHERE userID = @userID", connection);
                        command.Parameters.AddWithValue("@userID", this.id);

                        if (!string.IsNullOrEmpty(newUsername))
                        {
                            command.Parameters.AddWithValue("@username", newUsername);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@username", this.userName); // Keep the current username
                        }

                        if (!string.IsNullOrEmpty(newPassword))
                        {
                            command.Parameters.AddWithValue("@password", newPassword);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@password", this.password); // Keep the current password
                        }

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Profile updated successfully!");
                            if (!string.IsNullOrEmpty(newUsername)) this.userName = newUsername;
                            if (!string.IsNullOrEmpty(newPassword)) this.password = newPassword;
                        }
                        else
                        {
                            Console.WriteLine("Failed to update profile. Please try again.");
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("An error occurred while updating your profile: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("No changes were made to your profile.");
            }
        }

        public User(int id, string userName, string password)
        {
            this.id = id;
            this.userName = userName;
            this.password = password;
        }
        

    }
}
