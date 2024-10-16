﻿using Azure;
using HumanResources.Application.FileServices;
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
        private readonly IFileService _fileService;

        public EmployeeService(IGenericRepository<Employee> employeeRepository
            , IUnitOfWork unitOfWork
            , ApplicationDbContext context
            , IFileService fileService)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _context = context;
            _fileService = fileService;
        }

        public async Task<IEnumerable<Employee>> GetAll()
        {
            var data = _employeeRepository.GetAll(d => d.IsDeleted == false);
            return data;
        }
        public async Task Create(EmployeeDtoForAdd dto)
        {
            string GraduationCertificateUrl = "";
            string IdentityUrl = "";
            string PersonalImageUrl = "";
            if (dto.GraduationCertificateFile != null)
            {
                GraduationCertificateUrl = _fileService.UploadImage(dto.GraduationCertificateFile, "graduationCertificates");
            }
            if (dto.IdentityFile != null)
            {
                IdentityUrl = _fileService.UploadImage(dto.IdentityFile, "identity");
            }
            if (dto.PersonalImageFile != null)
            {
                PersonalImageUrl = _fileService.UploadImage(dto.PersonalImageFile, "personalImages");
            }
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
                GraduationCertificateUrl=GraduationCertificateUrl,
                IdentityUrl=IdentityUrl,
                PersonalImageUrl=PersonalImageUrl,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            };
            _employeeRepository.Add(newEmployee);
            _unitOfWork.SaveChanges();// Assuming SaveChangesAsync is implemented
                                      // After saving the employee, dynamically create a new table using the Code
  //          string tableName = $"Employee_{newEmployee.Code}";

  //          // Ensure that the table name is valid
  //          string createTableQuery = $@"
  //          CREATE TABLE [{tableName}] (
  //              Id INT PRIMARY KEY IDENTITY(1,1),
  //CheckInTime TIME, 
  //  CheckOutTime TIME,
  //  AttendanceDate DATE )
  //          ";

  //          await _context.Database.ExecuteSqlRawAsync(createTableQuery);
        }
        public async Task Update(Employee dto)
        {
            dto.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
            _employeeRepository.Update(dto);
            _unitOfWork.SaveChanges();// Assuming SaveChangesAsync is implemented
        }
        public async Task Delete(int id)
        {
            Employee employee = _employeeRepository.GetById(id);
            employee.DeletedAt = DateOnly.FromDateTime(DateTime.Now);
            employee.IsDeleted = true;
            _employeeRepository.Update(employee);
            _unitOfWork.SaveChanges();// Assuming SaveChangesAsync is implemented
        }

        public async Task<EmployeeDtoForShow> GetByIdForDetails(int id)
        {
            // var employee = _employeeRepository.GetById(id);
            var employee = _context.EmployeeTbl.Include("Department").FirstOrDefault(e=>e.Id==id);
            if (employee == null)
                return null;
            DateTime today = DateTime.Today;

            // Calculate the difference in years
            var data = new EmployeeDtoForShow
            {
                Code = employee.Code,
                Name = employee.Name,
                Address = employee.Address,
                Phone = employee.Phone,
                GrossSalary = Convert.ToInt32(employee.GrossSalary),
                Gender = employee.Gender.Value,
                CheckInTime = employee.CheckInTime?.ToString(),
                DateOfAppointment = employee.DateOfAppointment.ToString(),
                CheckOutTime = employee.CheckOutTime.ToString(),
                DepartmentName = employee.Department.Name,
                IdentityUrl = employee.IdentityUrl,
                GraduationCertificateUrl = employee.GraduationCertificateUrl,
                PersonalImageUrl = employee.PersonalImageUrl,

            };
            return data;
        }

        public Task<Employee> GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}