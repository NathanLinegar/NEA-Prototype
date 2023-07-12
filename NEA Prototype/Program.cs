using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

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
            Console.WriteLine("4) Not implemented yet");
        }
        static void ViewDataBase()
        {
            ViewDataBaseChoices();
            SQLiteConnection conn = new SQLiteConnection("Driver = {RotaSystemDataBase.db}");
        }
    }
} 
