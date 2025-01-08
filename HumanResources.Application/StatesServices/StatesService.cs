using Azure;
using HumanResources.Infrastructure.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HumanResources.Application.Dtos.StatisticsDto;

namespace HumanResources.Application.StatesServices
{
    public class StatesService: IStatesService
    { 
     private readonly ApplicationDbContext _context;
     private readonly UserManager<IdentityUser> _userManager;
    public StatesService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
            public async Task<States> GetStates()
    {
        States data = new States();
        // Initialize the properties
        data.Department = new State();
        data.Employee = new State();
        data.Week = new State();
        data.Bouns = new State();
        data.Loan = new State();
        var currentMonth = DateTime.UtcNow.Month;

            //Department
            data.Department.Count = _context.DepartmentTbl.Where(s => s.IsDeleted == false).Distinct().Count();
            // Get the Department count added in the current month
            decimal curentMonthCountDepartment = _context.DepartmentTbl.Count(s => s.CreatedAt.Month >= currentMonth);
            if (curentMonthCountDepartment != 0)
            {
                decimal DepartmentPercentage = (curentMonthCountDepartment / Convert.ToDecimal(data.Department.Count)) * 100;
                data.Department.Percentage = Math.Round(DepartmentPercentage, 2);
            }
            //Employee
            data.Employee.Count = _context.EmployeeTbl.Where(e=>e.IsDeleted==false).Count();
            // Get the Employee count added in the current month
            decimal curentMonthCountEmployee = _context.EmployeeTbl.Count(s => s.CreatedAt.Month >= currentMonth);
            if (curentMonthCountEmployee != 0)
            {
                decimal EmployeePercentage = (curentMonthCountEmployee / Convert.ToDecimal(data.Employee.Count)) * 100;
                data.Employee.Percentage = Math.Round(EmployeePercentage, 2);
            }

            //Week
            data.Week.Count = _context.WeekTbl.Distinct().Count();
            // Get the Week count added in the current month
            decimal curentMonthCountWeek = _context.WeekTbl.Count(s => s.CreatedDate.Value.Month >= currentMonth);
            if (curentMonthCountWeek != 0)
            {
                decimal WeekPercentage = (curentMonthCountWeek / Convert.ToDecimal(data.Week.Count)) * 100;
                data.Week.Percentage = Math.Round(WeekPercentage, 2);
            }


            //Bouns
            data.Bouns.Count = _context.BonusTbl.Where(s => s.IsDeleted == false).Count();
            // Get the Bouns count added in the current month
            decimal curentMonthCountBouns = _context.BonusTbl.Count(s => s.CreatedAt.Month >= currentMonth);
            if(curentMonthCountBouns !=0)
            {
                decimal BounsPercentage = (curentMonthCountBouns / Convert.ToDecimal(data.Bouns.Count)) * 100;
                data.Bouns.Percentage = Math.Round(BounsPercentage, 2);
            }
            //loan
            data.Loan.Count = _context.LoanTbl.Where(s => s.IsDeleted == false).Count();
            // Get the Bouns count added in the current month
            decimal curentMonthCountLoan = _context.BonusTbl.Count(s => s.CreatedAt.Month >= currentMonth);
            if (curentMonthCountLoan != 0)
            {
                decimal LoanPercentage = (curentMonthCountLoan / Convert.ToDecimal(data.Loan.Count)) * 100;
                data.Loan.Percentage = Math.Round(LoanPercentage, 2);
            }

            // Get the user count added in the current month

            return data;
    }
}
}