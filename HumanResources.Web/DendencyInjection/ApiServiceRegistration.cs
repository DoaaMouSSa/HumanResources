using HumanResources.Application.AuthServices;
using HumanResources.Application.DepartmentServices;

namespace HumanResources.Web.DendencyInjection
{
    public static class ApiServiceRegistration
    {
        public static void AddApiDependencies(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<IDepartmentService, DepartmentService>();

            // Other API-specific dependencies can be added here
        }
    }
}
