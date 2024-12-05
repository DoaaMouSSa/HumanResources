using HumanResources.Application.Dtos;
using HumanResources.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Application.WeekServices
{
    public interface IWeekService
    {
        Task<IEnumerable<WeekDto>> GetAll();
    }
}
