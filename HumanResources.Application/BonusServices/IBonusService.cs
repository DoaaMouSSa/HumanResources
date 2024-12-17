using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HumanResources.Application.Dtos.BonusDto;

namespace HumanResources.Application.BonusServices
{
    public interface IBonusService
    {
        Task<IEnumerable<BonusDtoForShow>> GetAll();
        Task Create(BonusDtoForAdd dto);
    }
}
