using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement
{
    public class ExceptionHandling
    {
        public static string StringHandling()
        {
            try
            {
                string input = Console.ReadLine();
                if (input.Length > 0)
                {
                    return input;

                }
                else
                {
                    throw new Exception("Value cannot be empty");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong :( {0}", e.Message);
                Console.WriteLine("=====================\n1. Try again\n2. Back to menu");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        Console.WriteLine("Enter a value again");
                        return StringHandling();
                    case "2":
                        if (Register_Login.currentUser == null)
                        {
                            Register_Login.DisplayMenu();
                        }
                        else
                        {
                            Register_Login.currentUser.DisplayMenu();
                        }
                        break;
                    default:
                        Console.WriteLine("Something went wrong :( ");
                        Console.WriteLine("=====================\n1. Try again\n2. Back to menu");
                        return StringHandling();
                }

                return null;
            }
        }

        public static int IntHandling()
        {
            try
            {
                int input = Convert.ToInt32(Console.ReadLine());
                //Enter
                return input;
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong :( {0}", e.Message);
                Console.WriteLine("=====================\n1. Try again\n2. Back to menu");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        Console.WriteLine("Enter a value again");
                        return IntHandling();
                    case "2":
                        if (Register_Login.currentUser == null)
                        {
                            Register_Login.DisplayMenu();
                        }
                        else
                        {
                            Register_Login.currentUser.DisplayMenu();
                        }
                        break;
                    default:
                        Console.WriteLine("Something went wrong :( ");
                        Console.WriteLine("=====================\n1. Try again\n2. Back to menu");
                        return IntHandling();

                }

                return 1;
            }
        }

        public static double DoubleHandling()
        {
            try
            {
                double input = Convert.ToDouble(Console.ReadLine());
                return input;
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong :( {0}", e.Message);
                Console.WriteLine("=====================\n1. Try again\n2. Back to menu");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        Console.WriteLine("Enter a value again");
                        return DoubleHandling();
                    case "2":
                        if (Register_Login.currentUser == null)
                        {
                            Register_Login.DisplayMenu();
                        }
                        else
                        {
                            Register_Login.currentUser.DisplayMenu();
                        }
                        break;
                    default:
                        Console.WriteLine("Something went wrong :( ");
                        Console.WriteLine("=====================\n1. Try again\n2. Back to menu");
                        return DoubleHandling();
                }

                return 0;
            }
        }

        public static DateTime DateHandling()
        {
            try
            {
                DateTime input = Convert.ToDateTime(Console.ReadLine());
                return input;
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong :( {0}", e.Message);
                Console.WriteLine("=====================\n1. Try again\n2. Back to menu");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        Console.WriteLine("Enter a value again");
                        return DateHandling();
                    case "2":
                        if (Register_Login.currentUser == null)
                        {
                            Register_Login.DisplayMenu();
                        }
                        else
                        {
                            Register_Login.currentUser.DisplayMenu();
                        }
                        break;
                    default:
                        Console.WriteLine("Something went wrong :( ");
                        Console.WriteLine("=====================\n1. Try again\n2. Back to menu");
                        return DateHandling();
                }

                return DateTime.Now;
            }
        }

        public static bool BoolHandling()
        {
            try
            {

                string input = Console.ReadLine();
                if (input.ToLower() == "y")
                {
                    return true;
                }
                else if (input.ToLower() == "n")
                {
                    return false;
                }
                else
                {
                    throw new Exception();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong :( {0}", e.Message);
                Console.WriteLine("=====================\n1. Try again\n2. Back to menu");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        Console.WriteLine("Enter yes (y) or no (n) y/n");
                        return BoolHandling();
                    case "2":
                        if (Register_Login.currentUser == null)
                        {
                            Register_Login.DisplayMenu();
                        }
                        else
                        {
                            Register_Login.currentUser.DisplayMenu();
                        }
                        break;
                    default:
                        Console.WriteLine("Something went wrong :( ");
                        Console.WriteLine("=====================\n1. Try again\n2. Back to menu");
                        return BoolHandling();
                }
                return false;
            }
        }
    }
}
