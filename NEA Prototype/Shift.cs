using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace NEA_Prototype
{
    public class Shift
    {
       public int shiftID;
       public DateTime shiftDay;
       public DateTime shiftStartTime;
       public DateTime shiftEndTime;
        public Shift(int ID,string day, string start, string end) 
        { 
            shiftID = ID;
            string dayFormat = "yyyy-MM-dd";
            shiftDay = DateTime.Parse(day);
            GetDateTimeFromString(start, end);
        }
        public Shift(int ID,DateTime day, string start, string end) 
        {
            shiftID = ID;
            shiftDay = day;
            GetDateTimeFromString(start, end);
        }
        private void GetDateTimeFromString(string start, string end)
        {
            string timeformat = "HH:mm";
            shiftStartTime = DateTime.ParseExact(start, timeformat, CultureInfo.InvariantCulture);
            shiftEndTime = DateTime.ParseExact(end, timeformat, CultureInfo.InvariantCulture);
            
        }
    }

    public class EmployeeShift
    {
        public int employeeID;
        public int shiftID;
        public string role;
        public EmployeeShift(int empId, int shiftId, string shiftRole) 
        { 
            employeeID = empId;
            shiftID = shiftId;
            role = shiftRole;
        }
    }
}
