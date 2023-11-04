using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data;
using System.Security.AccessControl;
using System.Security.Policy;
using System.Threading;
using System.Linq.Expressions;
using System.Globalization;
// for Prototype ive got to get the view database done, remove and add for employee table
namespace NEA_Prototype
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Employee> ListOfEmployees = new List<Employee>();
            int userIndex = -1;
            string userInput = "";
            GetListOfEmployees(ref ListOfEmployees);
            if (LoginMenu(ListOfEmployees, ref userIndex))
            {
                if (ListOfEmployees[userIndex].password == "proton1")
                {
                    ChangePassword(ref ListOfEmployees, userIndex);
                }
                while (userInput != "x")
                {
                    Console.Clear();
                    if (ListOfEmployees[userIndex].AccessType == "Owner" || ListOfEmployees[userIndex].AccessType == "Manager" || ListOfEmployees[userIndex].AccessType == "Admin")
                    {
                        AdminChoices(ListOfEmployees, userIndex, ref userInput);

                    }
                    else
                    {
                        EmployeeChoices(ListOfEmployees, userIndex, ref userInput);
                    }
                }
            }
            else
            {
                Console.WriteLine("The username/password was inputted wrong wrong too many times");
                Thread.Sleep(2000);
            }
            Console.WriteLine("Exiting");
            Thread.Sleep(500);

        }
        static void GetListOfEmployees(ref List<Employee> ListOfEmployees)
        {
            try
            {
                Employee emp;
                int empID, hoursworked;
                string forename, surname, emailAdd, password, accountType, phonenum, daysWorkable, contractType;
                string DOB, leaveStart, leaveEnd;
                double wage, accExtraShift;
                string query = "SELECT * FROM Employees";
                using (SQLiteConnection conn = new SQLiteConnection("Data Source = RotaSystemDataBase.db;Version=3;"))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    using (SQLiteDataReader rdr = cmd.ExecuteReader()) // need to fix this later on
                    {
                        while (rdr.Read())
                        {
                            empID = rdr.GetInt32(rdr.GetOrdinal("EmployeeID"));
                            forename = rdr.GetString(rdr.GetOrdinal("Forename"));
                            surname = rdr.GetString(rdr.GetOrdinal("Surname"));
                            emailAdd = rdr.GetString(rdr.GetOrdinal("Email"));
                            password = rdr.GetString(rdr.GetOrdinal("Password"));
                            accountType = rdr.GetString(rdr.GetOrdinal("AccessType"));
                            phonenum = rdr.GetString(rdr.GetOrdinal("PhoneNum"));
                            DOB = rdr.GetString(rdr.GetOrdinal("DOB"));
                            accExtraShift = rdr.GetDouble(rdr.GetOrdinal("AcceptExtraShift"));
                            contractType = rdr.GetString(rdr.GetOrdinal("ContractType"));
                            wage = rdr.GetDouble(rdr.GetOrdinal("Wage"));
                            daysWorkable = rdr.GetString(rdr.GetOrdinal("DaysCanWork"));
                            hoursworked = rdr.GetInt32(rdr.GetOrdinal("HoursWorked"));
                            leaveStart = rdr.GetString(rdr.GetOrdinal("LeaveStart"));
                            leaveEnd = rdr.GetString(rdr.GetOrdinal("LeaveEnd"));
                            emp = new Employee(empID, forename, surname, emailAdd, password, phonenum, contractType, daysWorkable, accountType, accExtraShift, wage, DOB, leaveStart, leaveEnd);
                            if (contractType == "Full Time")
                            {
                                emp = new FullTime(empID, forename, surname, emailAdd, password, phonenum, contractType, daysWorkable, accountType, accExtraShift, wage, DOB, leaveStart, leaveEnd);
                            }
                            else if (contractType == "Part Time")
                            {
                                emp = new PartTime(empID, forename, surname, emailAdd, password, phonenum, contractType, daysWorkable, accountType, accExtraShift, wage, DOB, leaveStart, leaveEnd);
                            }
                            else if (contractType == "Zero Hour")
                            {
                                emp = new ZeroHour(empID, forename, surname, emailAdd, password, phonenum, contractType, daysWorkable, accountType, accExtraShift, wage, DOB, leaveStart, leaveEnd);
                            }
                            ListOfEmployees.Add(emp);
                        }
                        conn.Close();
                    }


                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }  //gets the employee data off of the database specifically the Employee table
        static bool LoginMenu(List<Employee> ListOfEmployees, ref int UserIndex)
        {
            string inputtedEmailAddress, inputtedPassword;
            int numOfAttempts = 0;
            bool correctDetails = false;
            while (numOfAttempts < 5 && correctDetails == false)
            {
                Console.WriteLine("Input Email Address (not case sensitive)");
                inputtedEmailAddress = Console.ReadLine();
                Console.WriteLine("Input Password (case sensitive)");
                inputtedPassword = Console.ReadLine();
                for (int i = 0; i < ListOfEmployees.Count; i++)
                {
                    if (ListOfEmployees[i].emailAdd.ToLower() == inputtedEmailAddress.ToLower())
                    {
                        if (inputtedPassword == ListOfEmployees[i].password)
                        {
                            correctDetails = true;
                            UserIndex = i;
                        }
                    }
                }
                numOfAttempts += 1;
                Console.Clear();
            }
            Console.Clear();
            return correctDetails;
        } //Brings up a login menu where you log into your profile
        static void DisplayAdminChoices(List<Employee> ListOfEmployees, int userIndex)
        {
            Console.WriteLine("1) View your rota");
            Console.WriteLine("2) Request/Accept trade shift requests");
            Console.WriteLine("3) Request time off");
            Console.WriteLine("4) View entries in the database");
            Console.WriteLine("5) Add an entry/ entries to the database");
            Console.WriteLine("6) Remove an entry/ entries from the database");
            if (ListOfEmployees[userIndex].AccessType.ToLower() == "owner" || ListOfEmployees[userIndex].AccessType.ToLower() == "manager")
            {
                Console.WriteLine("9) Accept/deny time off requests");
            }
            Console.WriteLine("0) Settings");
            Console.WriteLine("x) exit program");
        }
        static void AdminChoices(List<Employee> ListOfEmployees, int userIndex, ref string userInput) //where accounts with admin privalleges or above can access all choices avaiable to them
        {
            DisplayAdminChoices(ListOfEmployees, userIndex);
            userInput = Console.ReadLine().ToLower().Trim();
            switch (userInput)
            {
                case "1":
                    ViewRota();
                    break;
                case "2":
                    //       TradeShift();
                    break;
                case "3":
                    //       RequestTimeOff();
                    break;
                case "4":
                    ViewEmployees(ListOfEmployees);
                    break;
                case "5":
                    //AddNewEmployee();
                    break;
                case "6":
                    //RemoveEmployee();
                    break;
                case "7":
                    //DealWithTimeOffRequests();
                    break;
                case "0":
                    //Settings();
                    break;
                case "x":
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    break;
            }
        }
        static void DisplayEmployeeChoices()
        {
            Console.WriteLine("1) View your rota");
            Console.WriteLine("2) Request/Accept trade shift requests");
            Console.WriteLine("3) Request time off");
            Console.WriteLine("0) Settings");
            Console.WriteLine("x) Exit program");


        }
        static void EmployeeChoices(List<Employee> ListOfEmployees, int userIndex, ref string userInput) // Brings up choices accessible to all employees
        {
            userInput = Console.ReadLine().ToLower().Trim();
            switch (userInput)
            {
                case "1":
                    ViewRota();
                    break;
                case "2":
                    //ShiftTrade()
                    break;
                case "3":
                    //Request time off
                    break;
                case "0":
                    //settings()
                    break;
                case "x":
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    break;

            }
        }
        static void ChangePassword(ref List<Employee> ListOfEmployees, int userIndex)
        {
            string newPasswordInput = "";
            string currentPassword = ListOfEmployees[userIndex].password;
            string oldPassword = "";
            if (ListOfEmployees[userIndex].password == "proton1")
            {
                Console.WriteLine("Default password detected.\nChange of password required");
            }
            while (oldPassword != currentPassword && currentPassword == "proton1" || currentPassword != "proton1" && newPasswordInput != "x")
            {
                Console.WriteLine("Input new password (input x to go back");
                newPasswordInput = Console.ReadLine();
                if (currentPassword == "proton1")
                {
                    oldPassword = currentPassword;
                }
                else
                {
                    Console.WriteLine("Input the current password");
                    oldPassword = Console.ReadLine();
                }
            }
            string updateQuery = $"UPDATE Employees SET password = @newpassword WHERE EmployeeID = @employeeID";
            ListOfEmployees[userIndex].password = newPasswordInput;
            NonQueryUpdateEmployeeTable(ListOfEmployees, userIndex, updateQuery);


        }
        static void NonQueryUpdateEmployeeTable(List<Employee> ListOfEmployees, int userIndex, string updateQuery)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source = RotaSystemDataBase.db; Version = 3;"))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@newpassword", ListOfEmployees[userIndex].password);
                        cmd.Parameters.AddWithValue("@employeeID", ListOfEmployees[userIndex].EmployeeID);
                        cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        } //Updates the Employee Table
        static void ViewRota()
        {

        }
        static void ViewEmployeeOptions()
        {
            Console.WriteLine("Which entries do you wish to be displayed");
            Console.WriteLine("1) All employees");
            Console.WriteLine("2) Employees with a certain name (fore/sur)");
            Console.WriteLine("3) Employees with IDs within a specified range");
            Console.WriteLine("4) Employees with a certain liscence");
            Console.WriteLine("5) Employees above a certain age");
            Console.WriteLine("6) All employees not on leave");
            Console.WriteLine("x) Go back");
        }
        static void ViewEmployees(List<Employee> ListOfEmployees)
        {
            string choice = "";
            while (choice != "x")
            {

                ViewEmployeeOptions();
                choice = Console.ReadLine();
                if (choice == "1")
                {
                    for (int i = 0; i < ListOfEmployees.Count; i++)
                    {
                        Console.WriteLine($"ID: {ListOfEmployees[i].EmployeeID}, Forename: {ListOfEmployees[i].forename}, Surname: {ListOfEmployees[i].surname}, Email: {ListOfEmployees[i].emailAdd}, phoneNum: {ListOfEmployees[i].phoneNum}, Contract Type: {ListOfEmployees[i].ContractType},\nDays they can work: {ListOfEmployees[i].daysString}, Accepts Extra Shifts: {ListOfEmployees[i].acceptExtraShifts}, Wage: {ListOfEmployees[i].Wage}, DOB: {ListOfEmployees[i].DOB}, On Leave: {ListOfEmployees[i].onLeave}");
                        Console.WriteLine("\n\n");
                    }
                }
            }

        }
    }

}
