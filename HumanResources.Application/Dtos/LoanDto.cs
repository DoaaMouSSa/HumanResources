using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Application.Dtos
{
    public class LoanDto
    {
        public class LoanDtoForAdd
        {
            [Required(ErrorMessage ="مبلغ السلفة مطلوب")]
            public decimal loan_amount { get; set; }
            [Required(ErrorMessage = " عدد الدفعات مطلوب")]
            //عدد الدفعات
            public int numberofpayment { get; set; }
            public decimal paid { get; set; }
            public decimal left { get; set; }
            public int EmployeeId { get; set; }     
        }
        public class LoanDtoForShow
        {
            public int Id { get; set; }
            //عدد الدفعات
            public int numberofpayment { get; set; }
            //قيمه الدفعة
            public decimal loan_amount { get; set; }
            //الواحده قيمه الدفعة
            public decimal payment_unit { get; set; }
            public decimal paid { get; set; }
            public decimal left { get; set; }
            public string added_date { get; set; }
          
            public bool IsDone { get; set; }
            public string done_date { get; set; }

            public string EmployeeName { get; set; }    
        }
    }
}
