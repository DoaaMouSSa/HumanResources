using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Application.Dtos
{
    public class AttendanceDto
    {
        public class AttendanceDtoForReport
        {
            public int Id { get; set; }
            public string weekDate { get; set; }
            public string Name { get; set; }
            public decimal Salary { get; set; }
            public int? attendanceDays { get; set; }
            public decimal? daySalary { get; set; }
            public decimal? hourSalary { get; set; }
            public decimal? delays { get; set; }
            public decimal? overtime { get; set; }
            public decimal? hoursWeek { get; set; }
            public decimal? discount { get; set; }
            public decimal? totalHour { get; set; }
            public int? netSalary { get; set; }
        }
    }
}
