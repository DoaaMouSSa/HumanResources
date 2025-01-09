using HumanResources.Application.AttendanceServices;
using HumanResources.Application.BonusServices;
using HumanResources.Application.DepartmentServices;
using HumanResources.Application.EmployeeServices;
using HumanResources.Application.FileServices;
using HumanResources.Application.LoanServices;
using HumanResources.Application.StatesServices;
using HumanResources.Application.WeekServices;

namespace HumanResources.Web.DendencyInjection
{
    public static class ApiServiceRegistration
    {
        public static void AddApiDependencies(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<IFileService, FileService>();

            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IWeekService, WeekService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<ILoanService, LoanService>();
            services.AddScoped<IBonusService, BonusService>();
            services.AddScoped<IDeductionService, DeductionService>();
            services.AddScoped<IStatesService, StatesService>();

            // Other API-specific dependencies can be added here
        }
    }
}
