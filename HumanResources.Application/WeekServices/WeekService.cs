using HumanResources.Application.Dtos;
using HumanResources.Domain.Entities;
using HumanResources.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Application.WeekServices
{
    public class WeekService : IWeekService
    {
        private readonly IGenericRepository<Week> _weekRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WeekService(IGenericRepository<Week> weekRepository
            , IUnitOfWork unitOfWork)
        {
            _weekRepository = weekRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<WeekDto>> GetAll()
        {
            var weeks = from q in _weekRepository.GetAllWithNoCondtion().OrderByDescending(w=>w.CreatedDateTime)
                        select new WeekDto
                        {
                            Id = (int)q.Id,
                            Date = q.Date,
                        };
            return weeks;
        }

    }
}
