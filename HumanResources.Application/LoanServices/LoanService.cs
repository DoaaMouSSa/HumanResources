﻿using HumanResources.Application.Dtos;
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
            Loan newLoan = new Loan
            {
                amount = dto.amount,
                Done = false,
                EmployeeId = dto.EmployeeId,
                numberofpayment=dto.numberofpayment,
                payment=dto.amount/dto.numberofpayment,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            };
            _loanRepository.Add(newLoan);
            _unitOfWork.SaveChanges();
        }

        public async Task<IEnumerable<LoanDtoForShow>> GetAll()
        {
            var data = _context.LoanTbl.Where(b => b.IsDeleted == false).Include("Employee")
                           .Select(q => new LoanDtoForShow
                           {
                               Id = q.Id,
                               amount = q.amount,
                               EmployeeName = q.Employee.Name,
                               numberofpayment=q.numberofpayment,
                               payment=q.payment,
                               IsDone = q.Done
                           }).AsEnumerable();
            return data;
        }
    }
}
