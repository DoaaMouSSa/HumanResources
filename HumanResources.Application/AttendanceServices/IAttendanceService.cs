using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Application.AttendanceServices
{
    public interface IAttendanceService
    {
        void CreateEveryWeek(IFormFile file);
    }
}
