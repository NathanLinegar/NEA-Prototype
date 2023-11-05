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
using System.IO;
using System.Diagnostics.Eventing.Reader;
using System.ComponentModel;
// for Prototype ive got to get the view database done, remove and add for employee table
namespace NEA_Prototype
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Employee> ListOfEmployees = new List<Employee>();
            List<string> Licences = new List<string>();
            List<string> Roles = new List<string>();
            int userIndex = -1;
            string userInput = "";
            GetListOfEmployees(ref ListOfEmployees); //Need to update to include Liscenses. May make it another subroutine
            GetLicencesAndRoles(ref Licences, ref Roles);
            if (LoginMenu(ListOfEmployees, ref userIndex))
            {
                if (ListOfEmployees[userIndex].password == "proton1")
                {
                    ChangePassword(ref ListOfEmployees, userIndex);
                }
                //Need an if to display a notification if there is a trade request pending.
                while (userInput != "x")
                {
                    Console.Clear();
                    if (ListOfEmployees[userIndex].AccessType == "Owner" || ListOfEmployees[userIndex].AccessType == "Manager" || ListOfEmployees[userIndex].AccessType == "Admin")
                    {
                        AdminChoices(ListOfEmployees, userIndex, ref userInput, Licences);

                    }
                    else
                    {
                        EmployeeChoices(ListOfEmployees, userIndex, ref userInput, Licences);
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
        static void GetLicencesAndRoles(ref List<string> Licences, ref List<string> Roles)
        {
            string licenceName = "";
            string roleName = "";
            using (StreamReader rdr = new StreamReader("Licences.txt"))
            {
                string line;
                while ((line = rdr.ReadLine()) != null)
                {
                   licenceName = line.Split(',')[0];
                    Licences.Add(licenceName);
                    roleName = line.Split(',')[1];
                    Roles.Add(roleName);
                }

            }
        }
        
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
                Console.WriteLine("7) Accept/deny time off requests");
            }
            Console.WriteLine("8) Run automatic rota system maker"); //Come up with a better name later
            Console.WriteLine("9) Edit Licenses file");
            Console.WriteLine("0) Settings");
            Console.WriteLine("x) exit program");
        }
        static void AdminChoices(List<Employee> ListOfEmployees, int userIndex, ref string userInput,  List<string> Licences) //where accounts with admin privalleges or above can access all choices avaiable to them
        {
            DisplayAdminChoices(ListOfEmployees, userIndex);
            userInput = Console.ReadLine().ToLower().Trim();
            switch (userInput)
            {
                case "1":
                    ViewRota();
                    break;
                case "2":
                    //TradeShift();
                    break;
                case "3":
                    //RequestTimeOff();
                    break;
                case "4":
                    ViewEmployees(ListOfEmployees);
                    break;
                case "5":
                    AddEmployee(ListOfEmployees, userIndex, Licences);
                    break;
                case "6":
                    //RemoveEmployee();
                    break;
                case "7":
                    //DealWithTimeOffRequests();
                    break;
                case "8":
                    //Rota system maker;
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
        static void EmployeeChoices(List<Employee> ListOfEmployees, int userIndex, ref string userInput, List<string> Licences) // Brings up choices accessible to all employees
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
        static void NonQueryUpdateEmployeeTable(List<Employee> ListOfEmployees, int userIndex, string updateQuery,string cellToChange, string change)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source = RotaSystemDataBase.db; Version = 3;"))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue(cellToChange, change);
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
        } //Updates the Employee Tables existing values

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
                Console.Clear();
                ViewEmployeeOptions();
                choice = Console.ReadLine();
                if (choice == "1")
                {
                    for (int i = 0; i < ListOfEmployees.Count; i++)
                    {
                        Console.WriteLine($"ID: {ListOfEmployees[i].EmployeeID}, Forename: {ListOfEmployees[i].forename}, Surname: {ListOfEmployees[i].surname}, Email: {ListOfEmployees[i].emailAdd}, phoneNum: {ListOfEmployees[i].phoneNum}, Contract Type: {ListOfEmployees[i].ContractType},\nDays they can work: {ListOfEmployees[i].daysString}, Accepts Extra Shifts: {ListOfEmployees[i].acceptExtraShifts}, Wage: {ListOfEmployees[i].Wage}, DOB: {ListOfEmployees[i].DOB}, On Leave: {ListOfEmployees[i].onLeave}");
                        Console.WriteLine("\n\n");
                    }
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                }
                if (choice == "2")
                {
                    string nameToFind = "";
                    Console.WriteLine("Input a fore/sur name");
                    nameToFind = Console.ReadLine();
                    for (int i = 0;i < ListOfEmployees.Count;i++)
                    {
                        if (ListOfEmployees[i].forename.ToLower() == nameToFind.ToLower() || ListOfEmployees[i].surname.ToLower() == nameToFind.ToLower())
                        {
                            Console.WriteLine($"ID: {ListOfEmployees[i].EmployeeID}, Forename: {ListOfEmployees[i].forename}, Surname: {ListOfEmployees[i].surname}, Email: {ListOfEmployees[i].emailAdd}, phoneNum: {ListOfEmployees[i].phoneNum}, Contract Type: {ListOfEmployees[i].ContractType},\nDays they can work: {ListOfEmployees[i].daysString}, Accepts Extra Shifts: {ListOfEmployees[i].acceptExtraShifts}, Wage: {ListOfEmployees[i].Wage}, DOB: {ListOfEmployees[i].DOB}, On Leave: {ListOfEmployees[i].onLeave}");
                            Console.WriteLine("\n\n");
                        }
                    }
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                }
                if (choice == "3")
                {
                    int smallestID, largestID;
                    Console.WriteLine("Input the smallest ID in the range");
                    smallestID = int.Parse(Console.ReadLine());
                    if (smallestID < 0)
                    {
                        smallestID = 0;
                    }   
                    Console.WriteLine("Input the largest ID in the range");
                    largestID = int.Parse(Console.ReadLine());
                    for (int i = 0; i < ListOfEmployees.Count(); i++)
                    {
                        if (ListOfEmployees[i].EmployeeID >= smallestID && ListOfEmployees[i].EmployeeID <= largestID)
                        {
                            Console.WriteLine($"ID: {ListOfEmployees[i].EmployeeID}, Forename: {ListOfEmployees[i].forename}, Surname: {ListOfEmployees[i].surname}, Email: {ListOfEmployees[i].emailAdd}, phoneNum: {ListOfEmployees[i].phoneNum}, Contract Type: {ListOfEmployees[i].ContractType},\nDays they can work: {ListOfEmployees[i].daysString}, Accepts Extra Shifts: {ListOfEmployees[i].acceptExtraShifts}, Wage: {ListOfEmployees[i].Wage}, DOB: {ListOfEmployees[i].DOB}, On Leave: {ListOfEmployees[i].onLeave}");
                            Console.WriteLine("\n\n");
                        }
                    }
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                }
                if (choice == "4")
                {
                    Console.WriteLine("Not done yet");
                }
                if (choice == "5") 
                {
                    bool validInput = false;
                    string dateString = "";
                    DateTime inputDate;
                    while (validInput == false)
                    {
                        try
                        {
                            Console.WriteLine("Input a date (dd/MM/yyyy) you want displayed employees born before or on that day");
                            dateString = Console.ReadLine();
                            inputDate = DateTime.ParseExact(dateString, "dd/MM/yyyy", null);
                            validInput = true;
                            for (int i = 0; i < ListOfEmployees.Count; i++)
                            {
                                if (ListOfEmployees[i].DOB <= inputDate)
                                {
                                    Console.WriteLine($"ID: {ListOfEmployees[i].EmployeeID}, Forename: {ListOfEmployees[i].forename}, Surname: {ListOfEmployees[i].surname}, Email: {ListOfEmployees[i].emailAdd}, phoneNum: {ListOfEmployees[i].phoneNum}, Contract Type: {ListOfEmployees[i].ContractType},\nDays they can work: {ListOfEmployees[i].daysString}, Accepts Extra Shifts: {ListOfEmployees[i].acceptExtraShifts}, Wage: {ListOfEmployees[i].Wage}, DOB: {ListOfEmployees[i].DOB}, On Leave: {ListOfEmployees[i].onLeave}");
                                    Console.WriteLine("\n\n");
                                }
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Please input a date in the format dd/MM/yyyy");
                        }
                    }
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                }
                if (choice == "6")
                {
                    for (int i = 0; i < ListOfEmployees.Count;i++)
                    {
                        if (ListOfEmployees[i].onLeave == false)
                        {
                            Console.WriteLine($"ID: {ListOfEmployees[i].EmployeeID}, Forename: {ListOfEmployees[i].forename}, Surname: {ListOfEmployees[i].surname}, Email: {ListOfEmployees[i].emailAdd}, phoneNum: {ListOfEmployees[i].phoneNum}, Contract Type: {ListOfEmployees[i].ContractType},\nDays they can work: {ListOfEmployees[i].daysString}, Accepts Extra Shifts: {ListOfEmployees[i].acceptExtraShifts}, Wage: {ListOfEmployees[i].Wage}, DOB: {ListOfEmployees[i].DOB}, On Leave: {ListOfEmployees[i].onLeave}");
                            Console.WriteLine("\n\n");
                        }
                    }
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                }
            }
        }
        static void AddEmployee(List<Employee> EmployeeList,int userIndex, List<string> Licences)
        {
            int empID, hoursworked = 0, licenceFromFileLine = 0;
            string forename, surname, emailAdd, password, accountType, phonenum, daysWorkable, contractType, lisenceInput = "", allDataCorrect;
            string DOB, leaveStart = "01-01-0001", leaveEnd = "01-01-001";
            double wage, accExtraShift;
            bool correctdata = false, onLeave = false;
            string addQuery = "INSERT INTO Employees (EmployeeID, Forename, Surname, Email, Password, AccessType, PhoneNum, DOB, AcceptExtraShift, ContractType, Wage, DaysCanWork, HoursWorked, OnLeave, LeaveStart, LeaveEnd) VALUES (@ID, @Fore, @Sur, @Email, @Password, @Access, @PhoneNum, @DOB, @AccExtraShift, @ContType, @Wage, @Days, @HoursWorked, @OnLeave, @LeaveS, @LeaveE)";
            string addQuery2 = "INSERT INTO Licences (LicenceID EmployeeID, LiscenseName) VALUES (@LicenceID,@EmpID, @Liscence)";
            
            empID = EmployeeList.Count;
            password = "proton1";
            List<string> AddLicences = new List<string>();
            do
            {
                Console.Clear();
                Console.WriteLine("Add Employee:");
                Console.WriteLine("Input forname");
                forename = Console.ReadLine().Trim();
                Console.WriteLine("Input surname");
                surname = Console.ReadLine().Trim();
                Console.WriteLine("Input the email address");
                emailAdd = Console.ReadLine().Trim();
                if (EmployeeList[userIndex].AccessType.ToLower() == "owner")
                {
                    Console.WriteLine("Input account type (Owner,Admin,Manager,Standard)");
                    accountType = Console.ReadLine().Trim();
                }
                else if (EmployeeList[userIndex].AccessType.ToLower() == "manager")
                {
                    Console.WriteLine("Input account type (Admin,Standard)");
                    accountType = Console.ReadLine().Trim();
                    if (accountType.ToLower() != "admin" || accountType.ToLower() != "standard")
                    {
                        accountType = "Standard";
                    }
                }
                else
                {
                    accountType = "Standard";
                }
                Console.WriteLine("Input phone number");
                phonenum = Console.ReadLine().Trim();
                Console.WriteLine("Input days they can work as a string e.g. Monday,Tuesday,Wednesday");
                daysWorkable = Console.ReadLine().Trim();
                Console.WriteLine("Input contract type (Full Time, Part Time, Zero Hour");
                contractType = Console.ReadLine();
                Console.WriteLine("Input DOB (dd-MM-yyyy)");
                DOB = Console.ReadLine().Trim();
                Console.WriteLine("Input wage (xx.xx)");
                wage = double.Parse(Console.ReadLine().Trim());
                Console.WriteLine("Input if they will accept extra shifts or not (0.0 for no) (1.0 for yes)");
                accExtraShift = double.Parse(Console.ReadLine().Trim());
                while (lisenceInput != "x")
                {
                    Console.WriteLine("Input a liscence the employee has (x to leave loop)");
                    lisenceInput = Console.ReadLine().Trim();
                    if (lisenceInput.ToLower() != "x")
                    {
                        AddLicences.Add(lisenceInput);
                    }
                }
                Console.WriteLine("\nIs all the data correct (y/n)");
                allDataCorrect = Console.ReadLine().Trim();
                if (allDataCorrect.ToLower() == "y")
                {
                    correctdata = true;
                } 
            } while (correctdata == false);
            
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source = RotaSystemDataBase.db; Version = 3;"))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(addQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", empID);
                        cmd.Parameters.AddWithValue("@Fore", forename );
                        cmd.Parameters.AddWithValue("@Sur", surname );
                        cmd.Parameters.AddWithValue("@Email", emailAdd);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Access", accountType);
                        cmd.Parameters.AddWithValue("@PhoneNum", phonenum);
                        cmd.Parameters.AddWithValue("@DOB", DOB);
                        cmd.Parameters.AddWithValue("@AccExtraShift", accExtraShift);
                        cmd.Parameters.AddWithValue("@ContType", contractType);
                        cmd.Parameters.AddWithValue("@Wage", wage);
                        cmd.Parameters.AddWithValue("@Days", daysWorkable);
                        cmd.Parameters.AddWithValue("@HoursWorked", hoursworked);
                        cmd.Parameters.AddWithValue("@OnLeave", onLeave);
                        cmd.Parameters.AddWithValue("@LeaveS", leaveStart);
                        cmd.Parameters.AddWithValue("@LeaveE", leaveEnd);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    conn.Open();
                    using (SQLiteCommand cmd2 = new SQLiteCommand(addQuery2, conn))
                    {
                        for (int i = 0; i < AddLicences.Count; i++)
                        {
                            for (int  j = 0; j < Licences.Count; j++)
                            {
                                if (AddLicences[i] == Licences[j])
                                {
                                    licenceFromFileLine = i;
                                    break;
                                }
                                else
                                {
                                    licenceFromFileLine = Licences.Count();
                                    AddToLicenceFile(AddLicences);
                                }                             
                            }
                            cmd2.Parameters.AddWithValue("@LicenceID", Licences[licenceFromFileLine]);
                            cmd2.Parameters.AddWithValue("@EmpID", empID);
                            cmd2.Parameters.AddWithValue("@Liscence", AddLicences[0]);
                        }
                        
                    }
                    conn.Close();
                    Console.WriteLine("Employee Successfully added");
                    Thread.Sleep(1000);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Failed to add employee");
                Thread.Sleep(1000);
            }

        }
        static void ViewRota()
        {

        }
        static void AddToLicenceFile(List<string> LicencesToAdd)
        {

        }
        static void ShiftTrade()
        {

        }
        static void Settings()
        {

        }
        static void ViewSettingChoices()
        {
            Console.WriteLine("1) Change forename");
            Console.WriteLine("2) Change surname");
            Console.WriteLine("3) Change email address");
            Console.WriteLine("4) Change Password");
            Console.WriteLine("5) Change Phone Number");
            Console.WriteLine(""); //need to add more of these.
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
            NonQueryUpdateEmployeeTable(ListOfEmployees, userIndex, updateQuery,"@newpassword", newPasswordInput);


        }
        static void RequestTimeOff()
        {

        }
    }

}
