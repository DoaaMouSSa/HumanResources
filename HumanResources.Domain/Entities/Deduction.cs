using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HumanResources.Domain.Enums.Enums;

namespace HumanResources.Domain.Entities
{
    public class Deduction : BaseEntity
    {
        public decimal amount { get; set; }
        public DeductionType DeductionType { get; set; }
        public bool Done { get; set; }
        public DateOnly? DoneDate { get; set; }

        // العلاقة مع الموظف
        public int EmployeeId { get; set; } // Foreign Key referencing Employee.id
        public Employee? Employee { get; set; } // Navigation Property    
    }
}
