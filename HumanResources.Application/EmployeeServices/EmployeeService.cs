using Azure;
using HumanResources.Domain.Entities;
using HumanResources.Domain.Interfaces;
using HumanResources.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly ApplicationDbContext _context;

        public EmployeeService(IGenericRepository<Employee> employeeRepository
            , IUnitOfWork unitOfWork
            , ApplicationDbContext context)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _context = context;
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
                Code = dto.Code,
                Name = dto.Name,
                DepartmentId = dto.DepartmentId,
                Address = dto.Address,
                BirthOfDate = dto.BirthOfDate,
                CheckInTime = dto.CheckInTime,
                CheckOutTime = dto.CheckOutTime,
                DateOfAppointment = dto.DateOfAppointment,
                ExperienceLevel = dto.ExperienceLevel,
                Gender = dto.Gender,
                Governorate = dto.Governorate,
                GrossSalary = dto.GrossSalary,
                JobPosition = dto.JobPosition,
                MaritalStatus = dto.MaritalStatus,
                Phone = dto.Phone,

                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            };
            _employeeRepository.Add(newEmployee);
            _unitOfWork.SaveChanges();// Assuming SaveChangesAsync is implemented
                                      // After saving the employee, dynamically create a new table using the Code
            string tableName = $"Employee_{newEmployee.Code}";

            // Ensure that the table name is valid
            string createTableQuery = $@"
            CREATE TABLE [{tableName}] (
                Id INT PRIMARY KEY IDENTITY(1,1),
  CheckInTime TIME, 
    CheckOutTime TIME,
    AttendanceDate DATE )
            ";

            await _context.Database.ExecuteSqlRawAsync(createTableQuery);
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

        public async Task<EmployeeDtoForShow> GetByIdForDetails(int id)
        {
            CultureInfo arabicCulture = new CultureInfo("ar-SA");

            var employee = _employeeRepository.GetById(id);

            if (employee == null)
                return null;

            var data = new EmployeeDtoForShow
            {
                Code = employee.Code,
                Name = employee.Name,
                Address = employee.Address,
                Phone = employee.Phone,
                CheckInTime = employee.CheckInTime?.ToString(),
                //Age = employee.(birthOfDate.Date > today.AddYears(-age)) ? age - 1 : age,
                DateOfAppointment = employee.DateOfAppointment.ToString(),
                CheckOutTime = employee.CheckOutTime.ToString(),
                 DepartmentName = employee.DateOfAppointment.ToString(),

            };
            return data;
        }

        public Task<Employee> GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}