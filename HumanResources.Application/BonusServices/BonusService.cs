using HumanResources.Application.Dtos;
using HumanResources.Domain.Entities;
using HumanResources.Domain.Interfaces;
using HumanResources.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Task Create(BonusDto.BonusDtoForAdd dto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BonusDto.BonusDtoForShow>> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
