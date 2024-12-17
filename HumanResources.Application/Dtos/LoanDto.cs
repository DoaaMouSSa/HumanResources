using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Application.Dtos
{
    public class LoanDto
    {
        public class LoanDtoForAdd
        {
            public decimal amount { get; set; }
            public int numberofpayment { get; set; }
            public bool finished { get; set; }
            public int? EmployeeId { get; set; }     
        }
        public class LoanDtoForShow
        {
            public decimal amount { get; set; }
            public int numberofpayment { get; set; }
            public bool finished { get; set; }
            public string EmployeeName { get; set; }    
        }
    }
}
