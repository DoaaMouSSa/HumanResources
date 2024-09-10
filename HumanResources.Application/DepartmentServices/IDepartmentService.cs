using HumanResources.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Application.DepartmentServices
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAll(int pageNumber, int pageSize);
        Task Create(Department dto);
        Task Update(Department dto);
        Task Delete(Department dto);
        Task<Department> GetById(int id);


    }
}
