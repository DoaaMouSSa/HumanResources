using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Domain.Entities
{
    public class Loan : BaseEntity
    {
        //عدد الدفعات
        public int numberofpayment { get; set; }
        //قيمه الدفعة
        public decimal loan_amount { get; set; }
        //الواحده قيمه الدفعة
        public decimal payment_unit { get; set; }
        public decimal paid { get; set; }
        public decimal left { get; set; }
        public bool Done { get; set; }
        public DateOnly? DoneDate { get; set; }

        // العلاقة مع الموظف
        public int EmployeeId { get; set; } // Foreign Key referencing Employee.id
        public Employee? Employee { get; set; } // Navigation Property    
    }
}
