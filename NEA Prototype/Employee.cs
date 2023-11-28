using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NEA_Prototype
{
    public class Employee
    {
        public int EmployeeID, extraShiftsWorked, contractedHours, maxhours, hoursWorked;
        public string forename, surname, emailAdd, password, phoneNum, AccessType, ContractType, daysString;
        public DateTime DOB, LeaveStart, LeaveEnd;
        public bool acceptExtraShifts, onLeave, requestedTimeOff, workingshift;
        public double Wage;
        public double shiftPriority;
        public string[] DaysCanWork = new string[7];
        public List<string> Qualifications = new List<string>();

        public Employee(int ID, string fore, string sur, string email, string pass, string phone, string contType, string days, string access, double AccextraShift, double pay, string DoB, string leaveStart, string leaveEnd)
        {
            EmployeeID = ID;
            forename = fore;
            surname = sur;
            emailAdd = email;
            password = pass;
            phoneNum = phone;
            AccessType = access;
            Wage = pay;
            ContractType = contType;
            daysString = days;
            maxhours = 40;
            requestedTimeOff = false;
            GetDateTimeFromString(DoB, leaveStart, leaveEnd);
            GetDaysWorkable(days);
            AcceptsExtraShifts(AccextraShift);
            IsOnleave();
            ContractedHours();
        }
        private void GetDaysWorkable(string days)
        {
            string day = "";
            int nextIndex = 0;
            for (int i = 0; i < days.Length; i++)
            {
                if (days[i] != ',')
                {
                    day += days[i];
                }
                else if (days[i] == ',' || days.Length == i)
                {
                    DaysCanWork[nextIndex] = day;
                    nextIndex++;
                }
            }
        }
        private void GetDateTimeFromString(string dob, string leaveS, string leaveE)
        {
            CultureInfo UKformat = new CultureInfo("en-GB");
            DOB = DateTime.Parse(dob);
            LeaveStart = DateTime.Parse(leaveS, UKformat);
            LeaveEnd = DateTime.Parse(leaveE, UKformat);
        }
        private void IsOnleave()
        {
            DateTime currentDay = DateTime.Today;
            if (currentDay <= LeaveEnd && currentDay >= LeaveStart)
            {
                onLeave = true;
            }
            else
            {
                onLeave = false;
            }
        }
        private void AcceptsExtraShifts(double acc)
        {
            if (acc == 0)
            {
                acceptExtraShifts = false;
            }
            else
            {
                acceptExtraShifts = true;
            }
        }
        public virtual void ContractedHours()
        {
            contractedHours = 35;
        }
        public virtual void Priority(string day)
        {
            if ((DaysCanWork.Count() == 1 ) && (DaysCanWork.Contains(day) == true))
            {
                shiftPriority = 1.0;
            }
            else if (hoursWorked >= contractedHours)
            {
                shiftPriority = 0.0;
            }
            else
            {
                shiftPriority = 1.0 - ((double)hoursWorked + 0.0001 / contractedHours + 0.0001);
            }
        }
        public bool HasRequestedTimeOff()
        {
            return requestedTimeOff;
        }
        public bool workingShift()
        {
            return workingshift;
        }
    }
    public class FullTime : Employee
    {
        public FullTime(int ID, string fore, string sur, string email, string pass, string phone, string ContType, string days, string access, double AccextraShift, double pay, string DoB, string leaveStart, string leaveEnd) : base(ID, fore, sur, email, pass, phone, ContType, days, access, AccextraShift, pay, DoB, leaveStart, leaveEnd) { }
        public override void ContractedHours()
        {
            contractedHours = 35;
        }
        public override void Priority(string day)
        {
            base.Priority(day);
        }
    }

    public class PartTime : Employee
    {
        public PartTime(int ID, string fore, string sur, string email, string pass, string phone, string ContType, string days, string access, double AccextraShift, double pay, string DoB, string leaveStart, string leaveEnd) : base(ID, fore, sur, email, pass, phone, ContType, days, access, AccextraShift, pay, DoB, leaveStart, leaveEnd) {ContractedHours(); }
        public override void ContractedHours()
        {
            contractedHours = 16;
        }
        public override void Priority(string day)
        {
            base.Priority(day);
        }
    }
    public class ZeroHour : Employee
    {
        public ZeroHour(int ID, string fore, string sur, string email, string pass, string phone, string ContType, string days, string access, double AccextraShift, double pay, string DoB, string leaveStart, string leaveEnd) : base(ID, fore, sur, email, pass, phone, ContType, days, access, AccextraShift, pay, DoB, leaveStart, leaveEnd) { ContractedHours(); }
        public override void ContractedHours()
        {
            contractedHours = 0;
        }
        public override void Priority(string day)
        {
            shiftPriority = 0.0;
        }
    }
}
