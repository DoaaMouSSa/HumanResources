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
using static HumanResources.Application.Dtos.DeductionDto;
using static HumanResources.Application.Dtos.LoanDto;
using static HumanResources.Domain.Enums.Enums;

namespace HumanResources.Application.LoanServices
{
    public class DeductionService : IDeductionService
    {
        
            private readonly IGenericRepository<Deduction> _deductionRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly ApplicationDbContext _context;

            public DeductionService(IGenericRepository<Deduction> deductionRepository
                , IUnitOfWork unitOfWork,
                ApplicationDbContext context)
            {
            _deductionRepository = deductionRepository;
                _unitOfWork = unitOfWork;
                _context = context;
            }
            public async Task Create(DeductionDtoForAdd dto)
        {
            Deduction newDeduction = null;

            if (dto.DeductionType==DeductionType.ساعات)
            {
                decimal grossSalary = _context.EmployeeTbl.Where(e => e.Id == dto.EmployeeId)
                    .FirstOrDefault().GrossSalary;
                decimal hourSalary = grossSalary / 48;
                 newDeduction = new Deduction
                {
                    hours =dto.amount,
                    amount = hourSalary*dto.amount,
                    Done = false,
                    EmployeeId = dto.EmployeeId,
                    DeductionType = dto.DeductionType,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                };
            }
          
            else if (dto.DeductionType == DeductionType.نقدى)
            {
                 newDeduction = new Deduction
                {
                    hours=0,
                    amount = dto.amount,
                    Done = false,
                    EmployeeId = dto.EmployeeId,
                    DeductionType = dto.DeductionType,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                };
            }
            else
            {
                newDeduction = new Deduction
                {
                    hours = 0,
                    amount = dto.amount,
                    Done = false,
                    EmployeeId = dto.EmployeeId,
                    DeductionType = dto.DeductionType,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                };
            }

                _deductionRepository.Add(newDeduction);
            _unitOfWork.SaveChanges();
        }

        public async Task<IEnumerable<DeductionDtoForShow>> GetAll()
        {
            var data = _context.DeductionTbl.Where(b => b.IsDeleted == false).Include("Employee")
                           .Select(q => new DeductionDtoForShow
                           {
                               Id = q.Id,
                               amount = q.amount,
                               hours=q.hours,
                               EmployeeName = q.Employee.Name,
                               DeductionType=q.DeductionType,
                               IsDone = q.Done
                           }).AsEnumerable();
            return data;
        }
    }
}
