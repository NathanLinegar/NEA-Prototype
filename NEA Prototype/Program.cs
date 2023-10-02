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
            List<Employee> ListOfEmployees = new List<Employee>();
            ListOfEmployees = GetListOfEmployees(ListOfEmployees);
            LoginMenu();
            AdminMainMenu();
            int choice = int.Parse(Console.ReadLine());
            switch(choice)
            {
                case 1:
                    ViewRota();
                    break;
                case 2:
                    ViewDataBase();
                    break;
                case 3:
                    //AddNewEmployee();
                    break;
                case 4:
                    //RemoveEmployee();
                    break;

            }
            Console.ReadKey();
        }
        static List<Employee> GetListOfEmployees(List<Employee>ListOfEmployees)
        {
            try
            {
                string query = "SELECT * FROM Employees";
                using (SQLiteConnection conn = new SQLiteConnection("Data Source = RotaSystemDataBase.db"))
                {
                    conn.Open();

                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {

                        }
                    }
                }
            }
        
            catch 
            { 
            
            
            }

            return ListOfEmployees ; 
        }
        static void LoginMenu()
        {
            //lmao i'll get this done sometime
        }
        static void AdminMainMenu()
        {
            Console.WriteLine("1) View your rota");
            Console.WriteLine("2) View entries in the database");
            Console.WriteLine("3) Add an entry to the database");
            Console.WriteLine("4) Remove an entry from the database");
        }
        static void ViewRota()
        {

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
