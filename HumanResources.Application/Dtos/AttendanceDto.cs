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
            public string Name { get; set; }
            public decimal Salary { get; set; }
        }
    }
}
