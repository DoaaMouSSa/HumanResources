using HumanResources.Application.Dtos;
using HumanResources.Domain.Entities;
using HumanResources.Domain.Interfaces;
using HumanResources.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HumanResources.Application.Dtos.BonusDto;
using static HumanResources.Application.Dtos.LoanDto;

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
            public async Task Create(LoanDtoForAdd dto)
        {
            if(dto.numberofpayment==1)
            {
                Loan newLoan = new Loan
                {
                    loan_amount = dto.loan_amount,
                    numberofpayment = dto.numberofpayment,
                    payment_unit = (dto.loan_amount / dto.numberofpayment),
                    paid=0,
                    left=0,
                    Done = false,
                    EmployeeId = dto.EmployeeId,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                };
                _loanRepository.Add(newLoan);
                _unitOfWork.SaveChanges();
            }
            else if (dto.numberofpayment > 1)
            {
                Loan newLoan = new Loan
                {
                    loan_amount = dto.loan_amount,
                    numberofpayment = dto.numberofpayment,
                    payment_unit = (dto.loan_amount / dto.numberofpayment),
                    paid = 0,
                    left = dto.loan_amount,
                    Done = false,
                    EmployeeId = dto.EmployeeId,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                };
                _loanRepository.Add(newLoan);
                _unitOfWork.SaveChanges();
            }

        }

        public async Task<IEnumerable<LoanDtoForShow>> GetAll()
        {
            var data = _context.LoanTbl.Where(b => b.IsDeleted == false).Include("Employee")
                           .Select(q => new LoanDtoForShow
                           {
                               Id = q.Id,
                               EmployeeName = q.Employee.Name,
                               numberofpayment = q.numberofpayment,
                               loan_amount = q.loan_amount,
                               payment_unit = q.payment_unit,
                              paid=q.paid,
                              left=q.left,
                               added_date = q.CreatedAt.ToString(),
                               IsDone = q.Done,
                               done_date = (q.DoneDate == null) ?"----------": q.DoneDate.ToString(),

                           }).AsEnumerable();
            return data;
        }
    }
}
