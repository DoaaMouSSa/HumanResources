using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HumanResources.Application.Dtos.StatisticsDto;

namespace HumanResources.Application.StatesServices
{
    public interface IStatesService
    {
        public Task<States> GetStates();
    }
}
