﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
// for Prototype ive got to get the view database done, remove and add for employee table
namespace NEA_Prototype
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Employee> ListOfEmployees = new List<Employee>();
            List<string> QualificationsList = new List<string>();
            List<string> Roles = new List<string>();
            List<Shift> Shifts = new List<Shift>();
            List<EmployeeShift> EmployeeShifts = new List<EmployeeShift>();
            int userIndex = -1;
            string userInput = "";
            GetEmployees(ref ListOfEmployees); 
            GetQualificationsAndRoles(ref QualificationsList, ref Roles);
            GetShiftsList(ref Shifts);
            Console.ReadKey();
            if (LoginMenu(ListOfEmployees, ref userIndex))
            {
                if (ListOfEmployees[userIndex].password == "proton1")
                {
                    ChangePassword(ref ListOfEmployees, userIndex);
                }
                //if (traderequestpending == true) {Console.WriteLine("employeeName wants to trade xxx shift");}
                while (userInput != "x")
                {
                    Console.Clear();
                    if (ListOfEmployees[userIndex].AccessType == "Owner" || ListOfEmployees[userIndex].AccessType == "Manager" || ListOfEmployees[userIndex].AccessType == "Admin")
                    {
                        AdminChoices(ListOfEmployees, userIndex, ref userInput, QualificationsList, Roles, Shifts, EmployeeShifts);

                    }
                    else
                    {
                        EmployeeChoices(ListOfEmployees, userIndex, ref userInput, Shifts, EmployeeShifts);
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
        static void GetEmployees(ref List<Employee> ListOfEmployees)
        {
            try
            {
                Employee emp;
                int empID, hoursworked;
                string forename, surname, emailAdd, password, accountType, phonenum, daysWorkable, contractType;
                string DOB, leaveStart, leaveEnd;
                double wage, accExtraShift;
                string query = "SELECT * FROM Employees";
                List<string> qualificationList = new List<string>();
                using (SQLiteConnection conn = new SQLiteConnection("Data Source = RotaSystemDataBase.db;Version=3;"))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    using (SQLiteDataReader rdr = cmd.ExecuteReader()) 
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
                            else
                            {
                                emp = new Employee(empID, forename, surname, emailAdd, password, phonenum, contractType, daysWorkable, accountType, accExtraShift, wage, DOB, leaveStart, leaveEnd);
                            }
                            ListOfEmployees.Add(emp);
                        }
                        conn.Close();
                        GetEmployeeQualifications(ref ListOfEmployees);
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }  //gets the employee data off of the database from the Employee table
        static void GetQualificationsAndRoles(ref List<string> Qualifications, ref List<string> Roles)
        {
            string QualificationName;
            string roleName;
            using (StreamReader rdr = new StreamReader("Qualifications.txt"))
            {

                string line;
                while ((line = rdr.ReadLine()) != null)
                {
                    QualificationName = line.Split(',')[0];
                    Qualifications.Add(QualificationName.ToLower());
                    roleName = line.Split(',')[1];
                    Roles.Add(roleName.ToLower());
                }
                rdr.Close();
            }
        } //Gets the valid roles (those in a text files) and the respective roles those Qualifications allow the employee to do
        static void GetEmployeeQualifications(ref List<Employee> EmployeeList)
        {
            int employeeID = 0;
            string qualification = "";
            using (SQLiteConnection conn = new SQLiteConnection("Data Source = RotaSystemDataBase.db;Version=3;"))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(conn);
                cmd.CommandText = "SELECT Qualifications.QualificationName, Employees.EmployeeID FROM Qualifications INNER JOIN Employees ON Qualifications.EmployeeID = Employees.EmployeeID";
                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        employeeID = rdr.GetInt32(rdr.GetOrdinal("EmployeeID"));
                        qualification = rdr.GetString(rdr.GetOrdinal("QualificationName"));
                        for (int i = 0; i < EmployeeList.Count; i++)
                        {
                            if (EmployeeList[i].EmployeeID == employeeID)
                            {
                                EmployeeList[i].Qualifications.Add(qualification.ToLower());
                            }
                        }
                    }
                }
            }
        } 
        static void GetShiftsList(ref List<Shift> shifts)
        {
            Shift shift;
            int shiftID;
            string shiftStart, shiftEnd, ShiftDay;
            string query = "SELECT * FROM Shifts";
            using (SQLiteConnection conn = new SQLiteConnection("Data Source = RotaSystemDataBase.db;Version=3;"))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        shiftID = rdr.GetInt32(rdr.GetOrdinal("ShiftID"));
                        shiftStart = rdr.GetString(rdr.GetOrdinal("ShiftStart"));
                        shiftEnd = rdr.GetString(rdr.GetOrdinal("ShiftEnd"));
                        ShiftDay = rdr.GetString(rdr.GetOrdinal("ShiftDay"));
                        shift = new Shift(shiftID, ShiftDay, shiftStart, shiftEnd);
                        shifts.Add(shift);
                    }
                }
                conn.Close();
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
        } 
        static void DisplayAdminChoices(List<Employee> ListOfEmployees, int userIndex)
        {
            Console.WriteLine("1) View your rota");
            Console.WriteLine("2) View full rota");
            Console.WriteLine("3) Request/Accept trade shift requests");
            Console.WriteLine("4) Request time off");
            Console.WriteLine("5) View employees in the database");
            Console.WriteLine("6) Add an employee to the database");
            Console.WriteLine("7) Remove an employee from the database");
            Console.WriteLine("8) Edit an entry from the database");
            Console.WriteLine("9) Run shift scheduler system");
            Console.WriteLine("10) Edit Licenses file");
            if (ListOfEmployees[userIndex].AccessType.ToLower() == "owner" || ListOfEmployees[userIndex].AccessType.ToLower() == "manager")
            {
                Console.WriteLine("11) Accept/deny time off requests");
            }
            Console.WriteLine("0) Settings");
            Console.WriteLine("x) exit program");
        } 
        static void AdminChoices(List<Employee> ListOfEmployees, int userIndex, ref string userInput, List<string> QualificationsList, List<string> Roles, List<Shift> Shifts, List<EmployeeShift> EmployeeShifts)
        {
            DisplayAdminChoices(ListOfEmployees, userIndex);
            userInput = Console.ReadLine().ToLower().Trim();
            switch (userInput)
            {
                case "1":
                    ViewRota(userIndex, Shifts, EmployeeShifts, ListOfEmployees);
                    break;
                case "2":
                    ViewFullRota(userIndex, Shifts, EmployeeShifts, ListOfEmployees);
                    break;
                case "3":
                    ShiftTrade();
                    break;
                case "4":
                    RequestTimeOff(ListOfEmployees, userIndex);
                    break;
                case "5":
                    ViewEmployees(ListOfEmployees,QualificationsList);
                    break;
                case "6":
                    AddEmployee(ref ListOfEmployees, userIndex, QualificationsList);
                    break; 
                case "7":
                    RemoveEmployee(ref ListOfEmployees);
                    break;
                case "8":
                    //EditQualificationTable();
                    break;
                case "9":
                    RotaSystemMaker(ref ListOfEmployees, ref QualificationsList, ref Roles, Shifts);
                    break;
                case "10":
                    EditQualificationsFile();
                    break;
                case "11":
                   DealWithTimeOffRequests(ListOfEmployees);
                    break;
                case "0":
                    Settings(ListOfEmployees, userIndex);
                    break;
                case "x":
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    break;
            }
        } //EDIT TABLE NEEDS TO BE MADE
        static void DisplayEmployeeChoices()
        {
            Console.WriteLine("1) View your rota");
            Console.WriteLine("2) View full rota");
            Console.WriteLine("3) Request/Accept trade shift requests");
            Console.WriteLine("4) Request time off");
            Console.WriteLine("0) Settings");
            Console.WriteLine("x) Exit program");


        }
        static void EmployeeChoices(List<Employee> ListOfEmployees, int userIndex, ref string userInput, List<Shift> Shifts, List<EmployeeShift> EmployeeShifts)
        {
            DisplayEmployeeChoices();
            userInput = Console.ReadLine().ToLower().Trim();
            switch (userInput)
            {
                case "1":
                    ViewRota(userIndex, Shifts, EmployeeShifts, ListOfEmployees);
                    break;
                case "2":
                    ViewFullRota(userIndex, Shifts, EmployeeShifts, ListOfEmployees); 
                    break;
                case "3":
                    ShiftTrade();
                    break;
                case "4":
                    RequestTimeOff(ListOfEmployees,userIndex);
                    break;
                case "0":
                    Settings(ListOfEmployees, userIndex);
                    break;
                case "x":
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    break;
            }
        }
        static void NonQueryUpdateEmployeeTable(List<Employee> ListOfEmployees, int userIndex, string updateQuery, string cellToChange, string change)
        {
            try
            {
                double booleanChange = 0.0;
                if (change == "1.0")
                {
                    booleanChange = 1.0;
                }
                if (change == "0.0")
                {
                    booleanChange = 0.0;
                }
                using (SQLiteConnection conn = new SQLiteConnection("Data Source = RotaSystemDataBase.db; Version = 3;"))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(updateQuery, conn))
                    {
                        if (change == "1.0" || change == "0.0")
                        {
                            cmd.Parameters.AddWithValue(cellToChange, booleanChange);
                            cmd.Parameters.AddWithValue("@employeeID", ListOfEmployees[userIndex].EmployeeID);
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(cellToChange, change);
                            cmd.Parameters.AddWithValue("@employeeID", ListOfEmployees[userIndex].EmployeeID);
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        } 

        static void ViewEmployeeOptions()
        {
            Console.WriteLine("Which entries do you wish to be displayed");
            Console.WriteLine("1) All employees");
            Console.WriteLine("2) Employees with a certain name (fore/sur)");
            Console.WriteLine("3) Employees with IDs within a specified range");
            Console.WriteLine("4) Employees with a certain licence");
            Console.WriteLine("5) Employees above a certain age");
            Console.WriteLine("6) All employees not on leave");
            Console.WriteLine("x) Go back");
        }
        static void ViewEmployees(List<Employee> ListOfEmployees,List<string> QualifcationList)
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
                        for (int j = 0; j < ListOfEmployees[i].Qualifications.Count(); j++)
                        {
                            Console.Write("Qualifications: " + ListOfEmployees[i].Qualifications[j]);
                        }
                        Console.WriteLine("\n\n");
                    }
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                }
                if (choice == "2")
                {
                    string nameToFind;
                    Console.WriteLine("Input a fore/sur name");
                    nameToFind = Console.ReadLine();
                    for (int i = 0; i < ListOfEmployees.Count; i++)
                    {
                        if (ListOfEmployees[i].forename.ToLower() == nameToFind.ToLower() || ListOfEmployees[i].surname.ToLower() == nameToFind.ToLower())
                        {
                            Console.WriteLine($"ID: {ListOfEmployees[i].EmployeeID}, Forename: {ListOfEmployees[i].forename}, Surname: {ListOfEmployees[i].surname}, Email: {ListOfEmployees[i].emailAdd}, phoneNum: {ListOfEmployees[i].phoneNum}, Contract Type: {ListOfEmployees[i].ContractType},\nDays they can work: {ListOfEmployees[i].daysString}, Accepts Extra Shifts: {ListOfEmployees[i].acceptExtraShifts}, Wage: {ListOfEmployees[i].Wage}, DOB: {ListOfEmployees[i].DOB}, On Leave: {ListOfEmployees[i].onLeave}");
                            for (int j = 0; j < ListOfEmployees[i].Qualifications.Count(); j++)
                            {
                                Console.Write("Qualifications: " + ListOfEmployees[i].Qualifications[j]);
                            }
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
                            for (int j = 0; j < ListOfEmployees[i].Qualifications.Count(); j++)
                            {
                                Console.Write("Qualifications: " + ListOfEmployees[i].Qualifications[j]);
                            }
                            Console.WriteLine("\n\n");
                        }
                    }
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                }
                if (choice == "4")
                {
                    string qualificationToView;
                    Console.WriteLine("Qualifications to choose from:");
                    for (int i = 0; i < QualifcationList.Count(); i++)
                    {
                        Console.WriteLine(QualifcationList[i]);
                    }
                    qualificationToView = Console.ReadLine();
                    for (int j =0; j < ListOfEmployees.Count(); j++)
                    {
                        for (int k = 0; k < ListOfEmployees[j].Qualifications.Count(); k++)
                        {
                            if (ListOfEmployees[j].Qualifications[j] == qualificationToView)
                            {
                                Console.WriteLine($"ID: {ListOfEmployees[j].EmployeeID}, Forename: {ListOfEmployees[j].forename}, Surname: {ListOfEmployees[j].surname}, Email: {ListOfEmployees[j].emailAdd}, phoneNum: {ListOfEmployees[j].phoneNum}, Contract Type: {ListOfEmployees[j].ContractType},\nDays they can work: {ListOfEmployees[j].daysString}, Accepts Extra Shifts: {ListOfEmployees[j].acceptExtraShifts}, Wage: {ListOfEmployees[j].Wage}, DOB: {ListOfEmployees[j].DOB}, On Leave: {ListOfEmployees[j].onLeave}");
                                Console.Write("Qualifications: " + ListOfEmployees[j].Qualifications[j]);
                                Console.WriteLine("\n\n");
                            }
                        }
                    }
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                } 
                if (choice == "5")
                {
                    bool validInput = false;
                    string dateString;
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
                                    for (int j = 0; j < ListOfEmployees[i].Qualifications.Count(); j++)
                                    {
                                        Console.Write("Qualifications: " + ListOfEmployees[i].Qualifications[j]);
                                    }
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
                    for (int i = 0; i < ListOfEmployees.Count; i++)
                    {
                        if (ListOfEmployees[i].onLeave == false)
                        {
                            Console.WriteLine($"ID: {ListOfEmployees[i].EmployeeID}, Forename: {ListOfEmployees[i].forename}, Surname: {ListOfEmployees[i].surname}, Email: {ListOfEmployees[i].emailAdd}, phoneNum: {ListOfEmployees[i].phoneNum}, Contract Type: {ListOfEmployees[i].ContractType},\nDays they can work: {ListOfEmployees[i].daysString}, Accepts Extra Shifts: {ListOfEmployees[i].acceptExtraShifts}, Wage: {ListOfEmployees[i].Wage}, DOB: {ListOfEmployees[i].DOB}, On Leave: {ListOfEmployees[i].onLeave}");
                            for (int j = 0; j < ListOfEmployees[i].Qualifications.Count(); j++)
                            {
                                Console.Write("Qualifications: " + ListOfEmployees[i].Qualifications[j]);
                            }
                            Console.WriteLine("\n\n");
                        }
                    }
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                }
            }
        } 
        static void AddEmployee(ref List<Employee> EmployeeList, int userIndex, List<string> QualificationsList)
        {
            int empID, hoursworked = 0;
            string forename, surname, emailAdd, password, accountType, phonenum, daysWorkable, contractType, licenceInput = "", allDataCorrect;
            string DOB, leaveStart = "01-01-0001", leaveEnd = "01-01-001";
            double wage, accExtraShift;
            bool correctdata = false, onLeave = false;
            string addQuery = "INSERT INTO Employees (EmployeeID, Forename, Surname, Email, Password, AccessType, PhoneNum, DOB, AcceptExtraShift, ContractType, Wage, DaysCanWork, HoursWorked, OnLeave, LeaveStart, LeaveEnd) VALUES (@ID, @Fore, @Sur, @Email, @Password, @Access, @PhoneNum, @DOB, @AccExtraShift, @ContType, @Wage, @Days, @HoursWorked, @OnLeave, @LeaveS, @LeaveE)";
            string addQuery2 = "INSERT INTO Qualifications (QualificationID, EmployeeID, QualificationName) VALUES (@QualificationID,@EmpID, @Qualification)";
            char userInput;
            empID = EmployeeList.Count;
            password = "proton1";
            List<string> AddQualifications = new List<string>();
            do
            {
                Console.Clear();
                Console.WriteLine("Add Employee:\n");
                Console.WriteLine("Input forname\n");
                forename = Console.ReadLine().Trim();
                Console.WriteLine("Input surname\n");
                surname = Console.ReadLine().Trim();
                Console.WriteLine("Input the email address\n");
                emailAdd = Console.ReadLine().Trim();
                if (EmployeeList[userIndex].AccessType.ToLower() == "owner")
                {
                    Console.WriteLine("Input account type (Owner,Admin,Manager,Standard)\n");
                    accountType = Console.ReadLine().Trim();
                }
                else if (EmployeeList[userIndex].AccessType.ToLower() == "manager")
                {
                    Console.WriteLine("Input account type (Admin,Standard)\n");
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
                Console.WriteLine("Input phone number\n");
                phonenum = Console.ReadLine().Trim();
                Console.WriteLine("Input days they can work as a string e.g. Monday,Tuesday,Wednesday\n");
                daysWorkable = Console.ReadLine().Trim();
                Console.WriteLine("Input contract type (Full Time, Part Time, Zero Hour)\n");
                contractType = Console.ReadLine();
                Console.WriteLine("Input DOB (dd-MM-yyyy)\n");
                DOB = Console.ReadLine().Trim();
                Console.WriteLine("Input wage (00.00)\n");
                wage = double.Parse(Console.ReadLine().Trim());
                try
                {
                    Console.WriteLine("Input if they will accept extra shifts or not (y/n)");
                    userInput = char.Parse(Console.ReadLine().Trim().ToLower());
                    if (userInput == 'y')
                    {
                        accExtraShift = 1.0;
                    }
                    else if (userInput == 'n')
                    {
                        accExtraShift = 0.0;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input");
                        accExtraShift = 0.0;
                    }
                }
                catch 
                {
                    accExtraShift= 0.0;
                }
                while (licenceInput != "x")
                {
                    Console.WriteLine("Input a licence the employee has (x to leave loop)");
                    Console.WriteLine();
                    Console.WriteLine("Current licences in program:");
                    for (int i = 0; i < QualificationsList.Count; i++)
                    {
                        Console.WriteLine(QualificationsList[i]);
                    }
                        licenceInput = Console.ReadLine().Trim();
                        if (licenceInput.ToLower() != "x")
                        {
                            AddQualifications.Add(licenceInput);
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
                        cmd.Parameters.AddWithValue("@Fore", forename);
                        cmd.Parameters.AddWithValue("@Sur", surname);
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
                    }
                    conn.Close();
                    Employee emp = new Employee(empID, forename, surname, emailAdd, password, phonenum, contractType, daysWorkable, accountType, accExtraShift, wage, DOB, leaveStart, leaveEnd);
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
                    EmployeeList.Add(emp);
                    for (int i = 0; i < AddQualifications.Count; i++)
                    {
                        InsertIntoQualificationsTable(AddQualifications[i],addQuery2,QualificationsList,empID);
                    }
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
            Console.ReadKey();

        } //Add try catch, make it so licenses can only be valid licences from txt file
        static void RemoveEmployee(ref List<Employee> EmployeeList)
        {
            Console.WriteLine("Input the email of the employee you wish to remove");
            string removeCondition = Console.ReadLine().Trim();
            int empID = -1;
            for (int i = 0;i < EmployeeList.Count;i++)
            {
                if (removeCondition.ToLower() == EmployeeList[i].emailAdd.ToLower())
                {
                    empID = i;
                }
            }
           
            using (SQLiteConnection conn = new SQLiteConnection("Data Source = RotaSystemDataBase.db; Version = 3;"))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "DELETE FROM EmployeeShifts WHERE EmployeeID = @EmployeeID;";
                    cmd.Parameters.AddWithValue("@EmployeeID", empID);
                    cmd.ExecuteNonQuery();
                }
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "DELETE FROM Qualifications WHERE EmployeeID = @EmployeeID;";
                    cmd.Parameters.AddWithValue("@EmployeeID", empID);
                    cmd.ExecuteNonQuery();
                }
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID;";
                    cmd.Parameters.AddWithValue("@EmployeeID", empID);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        } //Gotta have it remove the employee shift and their qualifications and put in someone who is qualified to fill that shift
        static void InsertIntoQualificationsTable(string qualification, string addQuery,List<string> QualificationsList, int empID)
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source = RotaSystemDataBase.db; Version = 3;"))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(addQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@QualificationID", QualificationsList.IndexOf(qualification));
                    cmd.Parameters.AddWithValue("@EmpID", empID);
                    cmd.Parameters.AddWithValue("@Qualificaion", qualification);
                    cmd.ExecuteNonQuery();               
                }
                conn.Close();
            }
        }
        static void EditQualificationsFile()
        {
            string userInput = "";
            while (userInput.ToLower().Trim() != "x")
            {
                Console.WriteLine("1)Add a qualification");
                Console.WriteLine("2)Remove a qualification");
                Console.WriteLine("x) Go back");
                if (userInput == "1")
                {
                    Console.WriteLine("Input a qualification name");
                    string qualification = Console.ReadLine();
                    Console.WriteLine("Input what role people with that qualification can do");
                    string roleAddition = Console.ReadLine();
                    using (StreamWriter sw = File.AppendText("Qualifications.Txt"))
                    { 
                      sw.WriteLine(qualification+","+roleAddition);
                    }
                }
                else if (userInput == "2")
                {
                    
                } 
                else if (userInput == "x")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input");
                }
            }
        } //Delete needs to be done still
        static void Settings(List<Employee> ListOfEmployees, int userIndex)
        {
            string userChoice = "";
            while (userChoice.ToLower().Trim() != "x")
            {
                ViewSettingChoices();
                userChoice = Console.ReadLine().ToLower().Trim();
                string updateQuery;
                switch (userChoice)
                {
                    case "1":
                        Console.WriteLine("Input what your forname should be changed to");
                        string newForname = Console.ReadLine().Trim();
                        updateQuery = $"UPDATE Employees SET Forename = @forename WHERE EmployeeID = @employeeID";
                        NonQueryUpdateEmployeeTable(ListOfEmployees, userIndex, updateQuery, "@forename", newForname);
                        ListOfEmployees[userIndex].forename = newForname;
                        break;
                    case "2":
                        Console.WriteLine("Input what your surname should be changed to");
                        string newSurname = Console.ReadLine().Trim();
                        updateQuery = $"UPDATE Employees SET Surname = @surname WHERE EmployeeID = @employeeID";
                        NonQueryUpdateEmployeeTable(ListOfEmployees, userIndex, updateQuery, "@surname", newSurname);
                        ListOfEmployees[userIndex].surname = newSurname;
                        break;
                    case "3":
                        Console.WriteLine("Input what your email should be changed to");
                        string newEmailAdd = Console.ReadLine().Trim();
                        updateQuery = $"UPDATE Employees SET Email = @email WHERE EmployeeID = @employeeID";
                        NonQueryUpdateEmployeeTable(ListOfEmployees, userIndex, updateQuery, "@email", newEmailAdd);
                        ListOfEmployees[userIndex].emailAdd = newEmailAdd;
                        break;
                    case "4":
                        ChangePassword(ref ListOfEmployees, userIndex);
                        break;
                    case "5":
                        Console.WriteLine("Input what your phone number should be changed to");
                        string newPhoneNum = Console.ReadLine().Trim();
                        updateQuery = $"UPDATE Employees SET PhoneNum = @phonenum WHERE EmployeeID = @employeeID";
                        NonQueryUpdateEmployeeTable(ListOfEmployees, userIndex, updateQuery, "@phonenum", newPhoneNum);
                        ListOfEmployees[userIndex].phoneNum = newPhoneNum;
                        break;
                    case "6":
                        try
                        {
                            string accExShifts;
                            updateQuery = $"UPDATE Employees SET AcceptExtraShift = @accExShifts WHERE EmployeeID = @employeeID";
                            Console.WriteLine("Change whether you accept extra shifts or not (y/n)");
                            char userInput = char.Parse(Console.ReadLine().ToLower().Trim());
                            if (userInput == 'y')
                            {
                                accExShifts = "1.0";
                            }
                            else
                            {
                                accExShifts = "0.0";
                            }
                            NonQueryUpdateEmployeeTable(ListOfEmployees, userIndex, updateQuery, "@accExShifts", accExShifts);
                        }
                        catch
                        {
                            Console.WriteLine("Invalid input setting to no as default");
                            string accExShifts = "0.0";
                            updateQuery = $"UPDATE Employees SET AcceptExtraShift = @accExShifts WHERE EmployeeID = @employeeID";
                            NonQueryUpdateEmployeeTable(ListOfEmployees, userIndex, updateQuery, "@accExShifts", accExShifts);
                        }
                        break;
                    case "x":
                        break;
                    default:
                        Console.WriteLine("Invalid input");
                        break;
                }
            }
        } 
        static void ViewSettingChoices()
        {
            Console.WriteLine("1) Change forename");
            Console.WriteLine("2) Change surname");
            Console.WriteLine("3) Change email address");
            Console.WriteLine("4) Change Password");
            Console.WriteLine("5) Change Phone Number");
            Console.WriteLine("6) Change acceptance of extra shifts");
            Console.WriteLine("x) Go back to main menu");
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
            while (oldPassword != currentPassword && currentPassword == "proton1" || currentPassword != "proton1" && newPasswordInput.ToLower().Trim() != "x")
            {
                Console.WriteLine("Input new password (input x to go cancel)");
                newPasswordInput = Console.ReadLine();
                if (newPasswordInput.ToLower().Trim() == "x" && currentPassword == "proton1")
                {
                    Console.WriteLine("Please change the password");
                }
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
            NonQueryUpdateEmployeeTable(ListOfEmployees, userIndex, updateQuery, "@newpassword", newPasswordInput);
            ListOfEmployees[userIndex].password = newPasswordInput;
        } 
        static void RequestTimeOff(List<Employee> ListOfEmployees, int userIndex) //File stores data as EmployeeID,LeaveStart,LeaveEnd,ReasonForLeave
        {
            Console.Clear();
            if (ListOfEmployees[userIndex].HasRequestedTimeOff() == false)
            {
                Console.WriteLine("Input the reason for leave");
                string reason = Console.ReadLine().Trim();
                Console.WriteLine("Input when you want the leave to start (dd-MM-yyyy)");
                string leaveStart = Console.ReadLine().Trim();
                Console.WriteLine("Input when you want the leave to end (dd-MM-yyyy)");
                string leaveEnd = Console.ReadLine().Trim();
                using (StreamWriter sw = File.AppendText("TimeOffRequests.txt"))
                { 
                 sw.WriteLine(userIndex + "," + leaveStart + "," + leaveEnd + "," + reason);
                }
                ListOfEmployees[userIndex].requestedTimeOff = true;
            }
            else
            {
                Console.WriteLine("You've already requested time off");
                Thread.Sleep(1000);
            }
        } //done
        static void DealWithTimeOffRequests(List<Employee> EmployeeList)
        {
            int userIndex = -1;
            DateTime leavestart = DateTime.Parse("01-01-0001");
            DateTime leaveend = DateTime.Parse("01-01-0001");
            string reason = "";
            using (StreamReader sr = new StreamReader("TimeOffRequests.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    userIndex = int.Parse(line.Split(',')[0]);
                    leavestart = DateTime.Parse(line.Split(',')[1]);
                    leaveend = DateTime.Parse(line.Split(',')[2]);
                    reason = line.Split(',')[3];

                    Console.WriteLine(EmployeeList[userIndex].forename + "wants leave starting" + leavestart + "for" + reason);
                    Console.WriteLine("Give them leave? (y/n)");
                    string choice = Console.ReadLine().ToLower().Trim();
                    if (choice == "y")
                    {
                        EmployeeList[userIndex].LeaveStart = leavestart;
                        EmployeeList[userIndex].LeaveEnd = leaveend;
                    }
                }
            }
            File.WriteAllText("TimeOffRequests.txt", string.Empty);
        } //done
        static void GetShiftRequirements(ref string dayStart, ref string dayEnd, ref List<int>numberOfEmployeesInRole, ref List<string>rolesForShifts)
        {
            using (StreamReader sr = new StreamReader("ShiftRequirements.txt"))
            {
                string line = "", tempstring = "";
                numberOfEmployeesInRole = new List<int>();
                rolesForShifts = new List<string>();
                int result = -1;
                while ((line = sr.ReadLine()) != null)
                {
                    dayStart = line.Split(',')[0];
                    dayEnd = line.Split(',')[1];
                    line = sr.ReadLine();
                    for (int i = 0;i< line.Length; i++)
                    {
                        if (line[i] != ',')
                        {
                            tempstring += line[i];
                        }
                        else if (line[i] == ',')
                        {
                            if (int.TryParse(tempstring, out result) == true)
                            {
                                numberOfEmployeesInRole.Add(result);
                                tempstring = "";
                            }
                            else if (int.TryParse(tempstring,out result) == false)
                            {
                                rolesForShifts.Add(tempstring);
                                tempstring = "";
                            }
                        }
                    }
                }
            }
        }
        static void RotaSystemMaker(ref List<Employee> EmployeeList, ref List<string> QualificationList, ref List<string> Roles, List<Shift> Shifts)
        {
            string dayStart = "", dayEnd = "", tempstring = "";
            int shiftlength = 6, shiftID = Shifts.Count;
            double startTime = 0.00, shiftchange = 0.00;
            List<int> numberOfEmployeesInRole = new List<int>();
            List<string> rolesForShifts = new List<string>(); //parallel lists
            int[] DayID = new int[] {1, 2, 3, 4, 5, 6, 7};
            string[] ShiftDay = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            DateTime weekBeginningDate = FirstDayOfWeek(DateTime.Now);
            DateTime dayOfShift;
            DateTime greatestDay = new DateTime(0001, 01, 01);
            GetShiftRequirements(ref dayStart, ref dayEnd, ref numberOfEmployeesInRole, ref rolesForShifts);
            for (int i = 0; i < dayStart.Length; i++)
            {
                if (dayStart[i] != ':')
                {
                    tempstring += dayStart[i];
                }
                else if (dayStart[i] == ':')
                {
                    tempstring += '.';
                }
            }
            
            startTime = double.Parse(tempstring);
            string shift1StartTime = dayStart;
            shiftchange = startTime + shiftlength;
            tempstring = "";
            
            for (int i = 0; i < shiftchange.ToString().Length; i++)
            {
                if (shiftchange.ToString()[i] == '.')
                {
                    tempstring += ":";
                }
                else
                {
                    tempstring += shiftchange.ToString()[i];
                }
            }
            string shift1EndTime = tempstring + 0;
            string shift2StartTime = shift1EndTime;
            string shift2EndTime = dayEnd;
            List<string> shiftStart = new List<string>();
            List<string> shiftEnd = new List<string>();
            shiftStart.Add(shift1StartTime); shiftStart.Add(shift2StartTime);
            shiftEnd.Add(shift1EndTime); shiftEnd.Add(shift2EndTime);

            Shift shift;
            int index = 0;
            int startShiftIndex = Shifts.Count();
            int endShiftIndex = Shifts.Count + 14;
            DateTime currentDate = new DateTime(0001, 01, 01);
            TimeSpan timespan = new TimeSpan(00, 00, 00);
            DateTime currentStartTime = DateTime.Parse(timespan.ToString());
            if (Shifts.Count > 0)
            {
                for(int i = 0; i < Shifts.Count;i++)
                {
                    if (Shifts[i].shiftDay > greatestDay)
                    {
                        weekBeginningDate = Shifts[i].shiftDay.AddDays(1);
                    }
                }
            }
            do
            {
                dayOfShift = weekBeginningDate.AddDays(index);
                for (int i = 0; i < shiftStart.Count; i++)
                {
                    AddToShiftTable(shiftID, dayOfShift, shiftStart[i], shiftEnd[i]);
                    shift = new Shift(shiftID, dayOfShift, shiftStart[i], shiftEnd[i]);
                    shiftID++;
                }
                index++;
            } while (ShiftDay.Count() > index);
            do
            {
                dayOfShift = weekBeginningDate.AddDays(index);

                for (int i = 0; i < Shifts.Count; i++)
                {
                    if (Shifts[i].shiftDay != currentDate || Shifts[i].shiftStartTime != currentStartTime)
                    {
                        currentDate = Shifts[i].shiftDay;
                        currentStartTime = Shifts[i].shiftStartTime;
                        foreach (Employee emp in EmployeeList)
                        {
                            emp.workingshift = false;
                        }
                    }
                    foreach (string neededRole in rolesForShifts)
                    {
                        if (numberOfEmployeesInRole[Roles.IndexOf(neededRole)] > 0)
                        {
                            Employee assignedEmployee = FindEmployeeForRole(EmployeeList, dayOfShift, neededRole);

                            if (assignedEmployee != null)
                            {
                                assignedEmployee.hoursWorked += shiftlength;
                                assignedEmployee.workingshift = true;
                                numberOfEmployeesInRole[Roles.IndexOf(neededRole)]--;
                                AddToEmployeeShiftTable(assignedEmployee.EmployeeID, neededRole, Shifts[i].shiftID);
                            }
                        }
                    }
                }
                index++;
            } while (ShiftDay.Count() > index);
        }
        static Employee FindEmployeeForRole(List<Employee> employees, DateTime day, string role)
        {
            double highestPriority = -1.0;
            Employee selectedEmployee = null;

            foreach (Employee emp in employees)
            {
                if (emp.DaysCanWork.Contains(day.DayOfWeek.ToString()) && emp.Qualifications.Contains(role.ToLower()) && emp.workingshift == false)
                {
                    if (emp.shiftPriority > highestPriority)
                    {
                        highestPriority = emp.shiftPriority;
                        selectedEmployee = emp;
                    }
                }
            }

            return selectedEmployee;
        }
        static void AddToEmployeeShiftTable(int employeeID, string role, int shiftID)
        {
            string addQuery = "INSERT INTO EmployeeShifts (EmployeeID,ShiftID,EmployeeRole) VALUES (@empID, @shiftID,@empRole)";
            using (SQLiteConnection conn = new SQLiteConnection("Data Source = RotaSystemDataBase.db; Version = 3;"))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(addQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@empID", employeeID);
                    cmd.Parameters.AddWithValue("@shiftID", shiftID);
                    cmd.Parameters.AddWithValue("@empRole", role);
                    cmd.ExecuteNonQuery();    
                }
                conn.Close();
            }
        }
      static void AddToShiftTable(int ShiftID, DateTime shiftDay, string shiftStart, string shiftEnd)
      {
            string addQuery = "INSERT INTO Shifts (ShiftID,ShiftDay,ShiftStart,ShiftEnd) VALUES (@ShiftID,@ShiftDay,@ShiftStart,@ShiftEnd)";
            using (SQLiteConnection conn = new SQLiteConnection("Data Source = RotaSystemDataBase.db; Version = 3;")) 
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(addQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ShiftID", ShiftID);
                    cmd.Parameters.AddWithValue("@ShiftDay", shiftDay);
                    cmd.Parameters.AddWithValue("@ShiftStart", shiftStart);
                    cmd.Parameters.AddWithValue("@ShiftEnd", shiftEnd);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            
      }
        static void ViewRota(int UserIndex,List<Shift>Shifts, List<EmployeeShift> empShift, List<Employee> Employees)
        {
            DateTime currentWeekBeginning = FirstDayOfWeek(DateTime.Now);
            for (int i = 0; i < 7; i++)
            {
                Console.Write(currentWeekBeginning.AddDays(i).DayOfWeek.ToString());
                foreach (EmployeeShift Shift in empShift)
                {
                    int ShiftID = Shift.shiftID;
                    if (Shift.employeeID == Employees[UserIndex].EmployeeID && Shifts[ShiftID].shiftDay == currentWeekBeginning.AddDays(i))
                    {
                        Console.Write(Shifts[ShiftID].shiftDay + " " + Shifts[ShiftID].shiftStartTime + " " + Shifts[ShiftID].shiftEndTime + " " + Shift.role);
                    }
                }
                Console.WriteLine();
            }
        }
        static void ViewFullRota(int UserIndex, List<Shift> Shifts, List<EmployeeShift> empShift, List<Employee> Employees)
        {
            DateTime currentWeekBeginning = FirstDayOfWeek(DateTime.Now);
            for (int i = 0; i < 7; i++)
            {
                foreach (EmployeeShift Shift in empShift)
                {
                    int ShiftID = Shift.shiftID;
                    int empID = Shift.employeeID;
                    if (Shifts[ShiftID].shiftDay == currentWeekBeginning.AddDays(i))
                    {
                        Console.Write(Shifts[ShiftID].shiftDay + " " + Shifts[ShiftID].shiftStartTime + " " + Shifts[ShiftID].shiftEndTime + " " + Employees[empID].forename + " " + Employees[empID].surname + " " + Shift.role);
                    }
                }
                Console.WriteLine();
            }
        } //same of one above but everything of importance to the rota system 
        static void ShiftTrade()
        {

        } //Needs shifts to be made  PersonWantingTrade, Onetheywanttotradewith, ShiftTheyHave,ShiftTheyWant





        // --------------------------------- COPIED CODE BELOW ---------------------------------
        static DateTime FirstDayOfWeek(DateTime datetoday)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = datetoday.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
            if (diff < 0)
                diff += 7;
            return datetoday.AddDays(-diff).Date;
        }
    }

}
