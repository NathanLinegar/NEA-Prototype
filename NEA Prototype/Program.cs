using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data;

namespace NEA_Prototype
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int UserInput;
            MainMenu();
            UserInput = int.Parse(Console.ReadLine());

            ViewDataBase();
            Console.ReadKey();
        }
        static void LoginMenu()
        {
            //lmao i'll get this done sometime
        }
        static void MainMenu()
        {
            Console.WriteLine("1) View Entries in the database");
            Console.WriteLine("2) Add an entry to the database");
            Console.WriteLine("3) Remove an entry from the database");
        }
        static void ViewDataBaseChoices()
        {
            Console.WriteLine("Which entries do you wish to be displayed");
            Console.WriteLine("1) All employees");
            Console.WriteLine("2) Employees with a certain name (fore/sur)");
            Console.WriteLine("3) Employees with IDs within a specified range");
        }
        static void ViewDataBase()
        {
            int Selection, rangeStartVal, rangeEndVal;
            string query = "", userinput, name = "";
            ViewDataBaseChoices();
            try
            {
                Selection = int.Parse(Console.ReadLine());

                if (Selection == 1)
                {
                    query = "SELECT * FROM Employees";
                }
                else if (Selection == 2)
                {
                    Console.WriteLine("Input a forename or surname");
                    userinput = Console.ReadLine();
                    for (int i = 0; i < userinput.Length; i++)
                    {
                        if (i == 0)
                        {
                            name += userinput[i].ToString().ToUpper();
                        }
                        else
                        {
                            name += userinput[i].ToString().ToLower();
                        }
                    }
                    query = $"SELECT * FROM Employees WHERE forename = '{name}' OR surname = '{name}'";
                }
                else if (Selection == 3)
                {

                }
                else
                {
                    Console.WriteLine("Invalid Input");
                }




                using (SQLiteConnection conn = new SQLiteConnection("Data Source = RotaSystemDataBase.db"))
                {
                    conn.Open();
                    float num;
                    num = (float)1067878/333;
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            Console.WriteLine("1");
                            Console.ReadKey();
         
                            Console.WriteLine(num);
                            Console.ReadKey();
                        }
                    }
                }
            }
            catch 
            { 
            
            
            }
        }
    }
} 
