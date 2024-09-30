using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Domain.Entities
{
    public class Attendance
    {
        public int Id { get; set; }

        // العلاقة مع الموظف
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public TimeSpan CheckInTime { get; set; }
        public TimeSpan CheckOutTime { get; set; }
        // New property to track the date of attendance
        public DateOnly AttendanceDate { get; set; }
    }
}
