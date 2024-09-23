using HumanResources.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Application.EmployeeServices
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAll();
        Task Create(Employee dto);
        Task Update(Employee dto);
        Task Delete(Employee dto);
        Task<Employee> GetById(int id);
    }
}
