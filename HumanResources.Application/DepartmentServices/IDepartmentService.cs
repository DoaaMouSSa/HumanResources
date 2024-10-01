using HumanResources.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HumanResources.Application.Dtos.DepartementDto;

namespace HumanResources.Application.DepartmentServices
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAll();
        Task Create(DepartmentDtoForAdd dto);
        Task Update(Department dto);
        Task Delete(Department dto);
        Task<Department> GetById(int id);


    }
}
