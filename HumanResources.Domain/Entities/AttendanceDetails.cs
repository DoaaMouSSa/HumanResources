using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Domain.Entities
{
    public class AttendancDetails
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }
        public int NumberOfAttandance { get; set; }
    }
}
