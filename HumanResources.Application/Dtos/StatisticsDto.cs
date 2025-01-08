using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Application.Dtos
{
    public class StatisticsDto
    {
        public class State
        {
            public int Count { get; set; }
            public decimal Percentage { get; set; }
        }
        public class States
        {
            public State Department { get; set; }
            public State Employee { get; set; }
            public State Week { get; set; }
            public State Bouns { get; set; }
            public State Loan { get; set; }
        }
    }
}
