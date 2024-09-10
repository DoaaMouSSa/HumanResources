using HumanResources.Domain.Interfaces;
using HumanResources.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static void AddInfrastructureDependencies(this IServiceCollection services)
        {

            // Register repositories and Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        }
    }
}
