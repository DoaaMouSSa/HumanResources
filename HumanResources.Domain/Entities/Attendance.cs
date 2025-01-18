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
        public int? EmployeeCode { get; set; } // Foreign Key referencing Employee.Code
        public Employee? Employee { get; set; } // Navigation Property
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? WorkingDays { get; set; }  
        public decimal? TotalWorkingHoursBeforeDelays { get; set; }
        public TimeSpan? TotalWorkingHoursTime { get; set; }
        public decimal? TotalWorkingHours { get; set; }
        public decimal hourSalary { get; set; }
        public decimal OverTimeHourSalary { get; set; }
        public decimal daySalary { get; set; }
        public decimal? SalaryBeforeAdditon { get; set; }
        public int? NetSalary { get; set; }
        public decimal? CalculatedSalary { get; set; }
        public decimal? CalculatedSalaryAfterAdditonals { get; set; }
        public decimal? OverTimeSalary { get; set; }
        public decimal? OverTimeHours { get; set; }
        public decimal? DelaysHours { get; set; }
        public TimeSpan? DelaysTime { get; set; }
        public int? WeekId { get; set; }
        public Week? Week { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deduction { get; set; }
        public decimal DeductionHours { get; set; }
        public decimal DeductionHoursAmount { get; set; }
        public decimal Loan { get; set; }
        public decimal Loanleft { get; set; }

        public List<AttendanceDetails>? AttendanceDetails { get; set; } = new List<AttendanceDetails>();


    }
}
