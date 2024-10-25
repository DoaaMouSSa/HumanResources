using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Domain.Entities
{
    public class Attendance
    {
        public int? Id { get; set; }

        // العلاقة مع الموظف
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? WorkingDays { get; set; }  
        public long? WorkingHours { get; set; }
        public ICollection<AttendanceDetails>? AttendanceDetails { get; set; } = new List<AttendanceDetails>();


    }
}
