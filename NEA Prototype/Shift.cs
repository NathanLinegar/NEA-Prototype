using System;
using System.Collections.Generic;
using System.Linq;
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
        public Shift() 
        { 
        
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
