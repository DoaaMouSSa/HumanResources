﻿using HumanResources.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HumanResources.Application.Dtos.EmployeeDto;

namespace HumanResources.Application.EmployeeServices
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAll();
        Task Create(EmployeeDtoForAdd dto);
        Task Update(Employee dto);
        Task Delete(int id);
        Task<Employee> GetById(int id);
        Task<EmployeeDtoForShow> GetByIdForDetails(int id);

    }
}
