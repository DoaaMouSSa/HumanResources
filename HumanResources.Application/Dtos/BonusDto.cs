using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Application.Dtos
{
    public class BonusDto
    {
        public class BonusDtoForAdd
        {
            public decimal amount { get; set; }
            public bool IsDone { get; set; }
            public int EmployeeId { get; set; } 
        }
        public class BonusDtoForShow
        {
            public int Id { get; set; }

            public decimal amount { get; set; }
            public string added_date { get; set; }

            public bool IsDone { get; set; }
            public string done_date { get; set; }
            public string EmployeeName { get; set; }
        }
    }
}
