using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA_Prototype
{
    internal class Employee
    {
        protected int EmployeeID;
        public string forename;
        public string surname;
        protected string emailAdd;
        protected string password;
        protected string phoneNum;
        protected DateTime DOB;
        protected bool acceptExtraShifts;
        protected float Wage;


        public Employee() { }
    }
}
