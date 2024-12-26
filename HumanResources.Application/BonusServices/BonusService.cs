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

namespace HumanResources.Application.BonusServices
{
    public class BonusService : IBonusService
    {
        private readonly IGenericRepository<Bonus> _bonusServiceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public BonusService(IGenericRepository<Bonus> bonusServiceRepository
            , IUnitOfWork unitOfWork,
            ApplicationDbContext context)
        {
            _bonusServiceRepository = bonusServiceRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }
        public async Task Create(BonusDtoForAdd dto)
        {
            Bonus newBonus = new Bonus
            {
               amount = dto.amount,
               Done=false,
               EmployeeId=dto.EmployeeId,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            };
            _bonusServiceRepository.Add(newBonus);
            _unitOfWork.SaveChanges();
        }

        public async Task<IEnumerable<BonusDtoForShow>> GetAll()
        {
            var data = _context.BonusTbl.Where(b=>b.IsDeleted==false).Include("Employee")
                .Select(q => new BonusDtoForShow
                {
                    Id = q.Id,  
                    amount=q.amount,
                    EmployeeName=q.Employee.Name,
                    IsDone=q.Done
                }).AsEnumerable();
            return data;
        }
    }
}
