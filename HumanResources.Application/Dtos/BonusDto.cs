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
            public bool finished { get; set; }
            public int? EmployeeCode { get; set; } 
        }
        public class BonusDtoForShow
        {
            public decimal amount { get; set; }
            public bool finished { get; set; }
            public string EmployeeName { get; set; }
        }
    }
}
