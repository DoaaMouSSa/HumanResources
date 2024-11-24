using HumanResources.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Application.AttendanceServices
{
    public class AttendanceService : IAttendanceService
    {
        public void CreateEveryWeek(IFormFile file)
        {
            throw new NotImplementedException();
        }
        Week week;
        Attendance attendance;
        List<AttendanceDetails> details;
        //private void CreateWeek(int weekId,int month,int year)
        //{
        //    week = new Week()
        //    {
        //        Code = weekId + "-" + month + "-" + year,
        //    };
        //}
        private decimal CalculateDaySalary()
        {
            decimal daysalary = 0;
            return daysalary;
        }
        private decimal CalculateHourSalary()
        {
            decimal hoursalary = 0;
            return hoursalary;
        }
        private void CreateAttendance(int attendanceId)
        {
            attendance = new Attendance()
            {
                daySalary = CalculateDaySalary(),
                hourSalary= CalculateHourSalary(),
                
            };
        }
        private void CreateAttendanceDetails()
        {

        }
    }
}
