using HumanResources.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HumanResources.Application.Dtos.LoanDto;

namespace HumanResources.Application.LoanServices
{
    public interface ILoanService
    {
        Task<IEnumerable<LoanDtoForShow>> GetAll();
        Task Create(LoanDtoForAdd dto);
    }
}
