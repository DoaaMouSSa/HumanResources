﻿using System;
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
        public double? TotalWorkingHoursBeforeDelays { get; set; }
        public double? TotalWorkingHours { get; set; }
        public decimal hourSalary { get; set; }
        public decimal OverTimeHourSalary { get; set; }
        public decimal daySalary { get; set; }
        public decimal? NetSalary { get; set; }
        public decimal? CalculatedSalary { get; set; }
        public decimal? OverTimeSalary { get; set; }
        public double? OverTimeHours { get; set; }
        public double? DelaysHours { get; set; }
        public TimeSpan? DelaysTime { get; set; }
        public int? WeekId { get; set; }
        public Week? Week { get; set; }
        public ICollection<AttendanceDetails>? AttendanceDetails { get; set; } = new List<AttendanceDetails>();


    }
}
