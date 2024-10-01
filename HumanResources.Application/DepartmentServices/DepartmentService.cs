using HumanResources.Domain.Entities;
using HumanResources.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HumanResources.Application.Dtos.DepartementDto;

namespace HumanResources.Application.DepartmentServices
{
    public class DepartmentService: IDepartmentService
    {
        private readonly IGenericRepository<Department> _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IGenericRepository<Department> departmentRepository
            , IUnitOfWork unitOfWork)
        {
            _departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Department>> GetAll()
        {
            var data = _departmentRepository.GetAll(d=>d.IsDeleted==false);
            return data;
        }
        public async Task Create(DepartmentDtoForAdd dto)
        {
            Department newDepartment = new Department()
            {
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                Name= dto.Name,
                Description= dto.Description,
                ImageUrl=null,
            };
             _departmentRepository.Add(newDepartment);
            _unitOfWork.SaveChanges();// Assuming SaveChangesAsync is implemented
        }
        public async Task Update(Department dto)
        {
            dto.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
            _departmentRepository.Update(dto);
            _unitOfWork.SaveChanges();// Assuming SaveChangesAsync is implemented
        }
        public async Task Delete(Department dto)
        {
            dto.DeletedAt = DateOnly.FromDateTime(DateTime.Now);
            dto.IsDeleted = true;
            _departmentRepository.Update(dto);
            _unitOfWork.SaveChanges();// Assuming SaveChangesAsync is implemented
        }

        public async Task<Department> GetById(int id)
        {
         var data=   _departmentRepository.GetById(id);
            return data;
        }
    }
}
