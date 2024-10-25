using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Domain.Entities
{
    public class AttendanceDetails
    {
        public int? Id { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public TimeSpan? WorkingHoursAday { get; set; }
        public DateOnly? AttendanceDate { get; set; }
        public int? AttendanceId { get; set; }
        public Attendance? Attendance { get; set; } 

    }
}
