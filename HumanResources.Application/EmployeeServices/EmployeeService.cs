using HumanResources.Domain.Entities;
using HumanResources.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HumanResources.Application.Dtos.EmployeeDto;

namespace HumanResources.Application.EmployeeServices
{
    public class EmployeeService: IEmployeeService
    {
        private readonly IGenericRepository<Employee> _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IGenericRepository<Employee> employeeRepository
            , IUnitOfWork unitOfWork)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Employee>> GetAll()
        {
            var data = _employeeRepository.GetAll(d => d.IsDeleted == false);
            return data;
        }
        public async Task Create(EmployeeDtoForAdd dto)
        {
            Employee newEmployee = new Employee
            {
                Id = dto.Id,
                Name = dto.Name,
                DepartmentId = dto.DepartmentId,
                //Address = dto.Address,
                //BirthOfDate = dto.BirthOfDate,
                //CheckInTime = dto.CheckInTime,
                //CheckOutTime = dto.CheckOutTime,
                //DateOfAppointment=dto.DateOfAppointment,
                //ExperienceLevel=dto.ExperienceLevel,
                //Gender=dto.Gender,
                //Governorate=dto.Governorate,
                //GrossSalary=dto.GrossSalary,
                //JobPosition=dto.JobPosition,
                //MaritalStatus=dto.MaritalStatus,
                //Phone=dto.Phone,
                
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            };
            _employeeRepository.Add(newEmployee);
            _unitOfWork.SaveChanges();// Assuming SaveChangesAsync is implemented
        }
        public async Task Update(Employee dto)
        {
            dto.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
            _employeeRepository.Update(dto);
            _unitOfWork.SaveChanges();// Assuming SaveChangesAsync is implemented
        }
        public async Task Delete(Employee dto)
        {
            dto.DeletedAt = DateOnly.FromDateTime(DateTime.Now);
            dto.IsDeleted = true;
            _employeeRepository.Update(dto);
            _unitOfWork.SaveChanges();// Assuming SaveChangesAsync is implemented
        }

        public async Task<Employee> GetById(int id)
        { 
            var data = _employeeRepository.GetById(id);
            return data;
        }
    }
}

