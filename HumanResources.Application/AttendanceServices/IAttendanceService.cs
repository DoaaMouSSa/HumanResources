using HumanResources.Application.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HumanResources.Application.Dtos.AttendanceDto;

namespace HumanResources.Application.AttendanceServices
{
    public interface IAttendanceService
    {
        List<AttendanceDtoForReport> GetForReport();
    }
}
