﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Domain.Entities
{
    public class Loan : BaseEntity
    {
        public decimal amount { get; set; }
        public int numberofpayment { get; set; }
        public decimal payment { get; set; }
        public bool Done { get; set; }
        // العلاقة مع الموظف
        public int EmployeeId { get; set; } // Foreign Key referencing Employee.id
        public Employee? Employee { get; set; } // Navigation Property    
    }
}
