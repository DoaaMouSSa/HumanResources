using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Domain.Entities
{
    public class Bonus:BaseEntity
    {
        public decimal amount { get; set; }
        public bool finished { get; set; }
        // العلاقة مع الموظف
        public int? EmployeeCode { get; set; } // Foreign Key referencing Employee.Code
        public Employee? Employee { get; set; } // Navigation Property    
    }
}
