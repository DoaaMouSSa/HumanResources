using HumanResources.Application.Dtos;
using HumanResources.Domain.Entities;
using HumanResources.Domain.Interfaces;
using HumanResources.Infrastructure.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HumanResources.Application.Dtos.AttendanceDto;

namespace HumanResources.Application.AttendanceServices
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IGenericRepository<Attendance> _attendanceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public AttendanceService(IGenericRepository<Attendance> attendanceRepository
            , IUnitOfWork unitOfWork,
            ApplicationDbContext context)
        {
            _attendanceRepository = attendanceRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public List<AttendanceDtoForReport> GetForReport(int weekId)
        {
            List<AttendanceDtoForReport> attendanceDtoForReports = (from q in _context.AttendanceTbl.Where(q=>q.WeekId==weekId).Include("Employee").Include("Week")
                       select new AttendanceDtoForReport
                       {
                           weekDate=q.Week.CreatedDate.ToString(),
                           Id = (int)q.Id,
                           Name = q.Employee.Name,
                           Salary = q.Employee.GrossSalary,
                           attendanceDays=q.WorkingDays,
                           daySalary=q.daySalary,
                           hourSalary=q.hourSalary,
                           delays=q.DelaysHours,
                           discount=0,
                           hoursWeek=q.TotalWorkingHours,
                           overtime=q.OverTimeHours,
                           totalHour=q.TotalWorkingHours,
                           netSalary=q.NetSalary,
                       }).ToList();
            return attendanceDtoForReports;
        }
    }
}
