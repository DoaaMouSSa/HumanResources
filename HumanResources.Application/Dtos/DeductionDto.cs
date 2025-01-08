using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HumanResources.Domain.Enums.Enums;

namespace HumanResources.Application.Dtos
{
    public class DeductionDto
    {
        public class DeductionDtoForAdd
        {
            public decimal amount { get; set; }
            public DeductionType DeductionType { get; set; }
            public bool Done { get; set; }
            public int EmployeeId { get; set; }
        }
        public class DeductionDtoForShow
        {
            public int Id { get; set; }
            public decimal amount { get; set; }
            public DeductionType DeductionType { get; set; }
            public string added_date { get; set; }

            public bool IsDone { get; set; }
            public string done_date { get; set; }
            public string EmployeeName { get; set; }
        }
    }
}
