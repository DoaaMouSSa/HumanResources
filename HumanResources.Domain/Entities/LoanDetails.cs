using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Domain.Entities
{
    public class LoanDetails:BaseEntity
    {
        public decimal paid { get; set; }
        public decimal left { get; set; }
        public DateOnly paymentDate { get; set; }
        public int? LoanId { get; set; }
        public Loan? Loan { get; set; }
    }
}
