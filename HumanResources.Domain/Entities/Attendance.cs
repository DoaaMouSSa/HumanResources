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
        public TimeSpan? WorkingHoursTime { get; set; }
        public decimal hourSalary { get; set; }
        public decimal daySalary { get; set; }
        public decimal? NetSalary { get; set; }
        //public decimal? deductionSalary { get; set; }
        //public decimal? OverTimeSalary { get; set; }
        //public long? DelaysHours { get; set; }
        //public TimeSpan? DelaysTime { get; set; }
        //public long? OverTimeHours { get; set; }
        //public TimeSpan? OverTime { get; set; }
        public ICollection<AttendanceDetails>? AttendanceDetails { get; set; } = new List<AttendanceDetails>();


    }
}
