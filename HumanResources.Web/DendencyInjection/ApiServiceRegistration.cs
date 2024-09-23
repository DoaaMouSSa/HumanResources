using HumanResources.Application.AuthServices;
using HumanResources.Application.DepartmentServices;
using HumanResources.Application.EmployeeServices;

namespace HumanResources.Web.DendencyInjection
{
    public static class ApiServiceRegistration
    {
        public static void AddApiDependencies(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEmployeeService, EmployeeService>();

            // Other API-specific dependencies can be added here
        }
    }
}
