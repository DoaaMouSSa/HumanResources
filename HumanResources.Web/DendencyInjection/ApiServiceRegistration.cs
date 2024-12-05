using HumanResources.Application.AttendanceServices;
using HumanResources.Application.AuthServices;
using HumanResources.Application.DepartmentServices;
using HumanResources.Application.EmployeeServices;
using HumanResources.Application.FileServices;
using HumanResources.Application.WeekServices;

namespace HumanResources.Web.DendencyInjection
{
    public static class ApiServiceRegistration
    {
        public static void AddApiDependencies(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IFileService, FileService>();

            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IWeekService, WeekService>();
            services.AddScoped<IAttendanceService, AttendanceService>();

            // Other API-specific dependencies can be added here
        }
    }
}
