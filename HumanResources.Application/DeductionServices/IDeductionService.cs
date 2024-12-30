using HumanResources.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HumanResources.Application.Dtos.DeductionDto;
using static HumanResources.Application.Dtos.LoanDto;

namespace HumanResources.Application.LoanServices
{
    public interface IDeductionService
    {
        Task<IEnumerable<DeductionDtoForShow>> GetAll();
        Task Create(DeductionDtoForAdd dto);
    }
}
