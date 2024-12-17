using HumanResources.Application.Dtos;
using HumanResources.Domain.Entities;
using HumanResources.Domain.Interfaces;
using HumanResources.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Application.LoanServices
{
    public class LoanService : ILoanService
    {
        
            private readonly IGenericRepository<Loan> _loanRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly ApplicationDbContext _context;

            public LoanService(IGenericRepository<Loan> loanRepository
                , IUnitOfWork unitOfWork,
                ApplicationDbContext context)
            {
            _loanRepository = loanRepository;
                _unitOfWork = unitOfWork;
                _context = context;
            }
            public Task Create(LoanDto.LoanDtoForAdd dto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LoanDto.LoanDtoForShow>> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
