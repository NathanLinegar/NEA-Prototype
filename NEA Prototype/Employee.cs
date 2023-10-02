using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA_Prototype
{
    public class Employee              //Parent class that defines the types of Employees
    {
        protected int EmployeeID;
        protected string forename, surname, emailAdd, password, phoneNum,AccessType, ContractType;
        protected DateTime DOB;
        protected bool acceptExtraShifts, onLeave;
        protected float Wage;
        protected string[] DaysCanWork = new string[7];
        protected DateTime LeaveStart, LeaveEnd;
        public Employee(int ID, string fore, string sur, string email, string pass,string AccType, string phone, DateTime DoB, bool extraShift,string ContType, float pay, string days)
        {
            EmployeeID = ID;
            forename = fore;
            surname = sur;
            emailAdd = email;
            password = pass;
            phoneNum = phone;
            DOB = DoB;
            AccessType = AccType;
            ContractType = ContType;
            acceptExtraShifts = extraShift;
            Wage = pay;
            string day = "";
            int nextIndex = 0;
            for (int i = 0; i < days.Length;i++)
            {
                if (days[i] != ',')
                {
                    day += days[i];
                }
                else if (days[i] == ',')
                {
                    DaysCanWork[nextIndex] = day;
                    nextIndex++;
                }
            }

        }
    }
   


}
