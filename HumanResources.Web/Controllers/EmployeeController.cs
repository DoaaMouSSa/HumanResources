using HumanResources.Application.DepartmentServices;
using HumanResources.Application.Dtos;
using HumanResources.Application.EmployeeServices;
using HumanResources.Domain.Entities;
using HumanResources.Infrastructure.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static HumanResources.Application.Dtos.EmployeeDto;
using static HumanResources.Domain.Enums.Enums;

namespace HumanResources.Web.Controllers
{
    [Authorize]

    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly ApplicationDbContext _context;

        public EmployeeController(IEmployeeService employeeService,
            IDepartmentService departmentService,
            ApplicationDbContext context)
        {
            _employeeService = employeeService;
            _departmentService= departmentService;
            _context= context;
        }

     
        public async Task<IActionResult> Index(int departmentId,int pageIndex = 1, int pageSize = 5)
        {
            IQueryable<EmployeeDtoForTable> query;
            if (departmentId==0)
            {
                query = _context.EmployeeTbl
      .Include(e => e.Department) // Include related Department
      .Where(e => e.IsDeleted == false) // Filter out deleted employees
      .Select(e => new EmployeeDtoForTable // Project to EmployeeDtoForTable
      {
          Id = e.Id,
          Code = e.Code,
          Name = e.Name,
          Phone = e.Phone,
          Gender = e.Gender,
          Department = e.Department.Name,
          GrossSalary = e.GrossSalary
      })
      .AsQueryable();

            }
            else
            {
                query = _context.EmployeeTbl
       .Include(e => e.Department) // Include related Department
       .Where(e => e.IsDeleted == false) // Filter out deleted employees
       .Where(e=>e.DepartmentId==departmentId)
       .Select(e => new EmployeeDtoForTable // Project to EmployeeDtoForTable
       {
           Id = e.Id,
           Code = e.Code,
           Name = e.Name,
           Phone = e.Phone,
           Gender = e.Gender,
           Department = e.Department.Name,
           GrossSalary = e.GrossSalary
       })
       .AsQueryable();
            }
            var totalCount = query.Count();
            var items =await query.Skip((pageIndex - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            var paginatedList = new PaginatedList<EmployeeDtoForTable>(items, totalCount, pageIndex, pageSize);
            IEnumerable<Department> departments = await _departmentService.GetAll();
            ViewData["DepartmentLst"] = new SelectList(departments, "Id", "Name");
            if (departmentId != 0)
            {
                ViewData["SelectedDepartmentName"] = _context.DepartmentTbl.FirstOrDefault(d => d.Id == departmentId).Name;

            }
            return View(paginatedList);
        }
        public async Task<IActionResult> GetEmployeesByDepartment(int departmentId, int pageIndex = 1, int pageSize = 5)
        {
            var query = (from q in _context.EmployeeTbl
                         where q.IsDeleted == false
                         && q.DepartmentId== departmentId
                         select new EmployeeDtoForTable
                         {
                             Name = q.Name,
                             Phone = q.Phone,
                             Gender = q.Gender,
                             Department = q.Department.Name,
                             GrossSalary = q.GrossSalary,
                         }).AsEnumerable();
            var totalCount = query.Count();
            var items = query.Skip((pageIndex - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();

            var paginatedList = new PaginatedList<EmployeeDtoForTable>(items, totalCount, pageIndex, pageSize);
            return View(paginatedList);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["Gender"]=new SelectList(Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList());
            ViewData["SalaryFormula"] =new SelectList(Enum.GetValues(typeof(SalaryFormula)).Cast<SalaryFormula>().ToList());
            ViewData["ExperienceLevel"] =new SelectList(Enum.GetValues(typeof(ExperienceLevel)).Cast<ExperienceLevel>().ToList());
            ViewData["MaritalStatus"] =new SelectList(Enum.GetValues(typeof(MaritalStatus)).Cast<MaritalStatus>().ToList());
            ViewData["Governorate"] =new SelectList(Enum.GetValues(typeof(Governorate)).Cast<Governorate>().ToList());
            ViewData["JobPosition"] =new SelectList(Enum.GetValues(typeof(JobPosition)).Cast<JobPosition>().ToList());
            IEnumerable<Department> departments = await _departmentService.GetAll();
            ViewData["DepartmentLst"] = new SelectList(departments, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(EmployeeDtoForAdd dto)
        {

            if (ModelState.IsValid)
            {
                // Check for duplicate code
                var isDuplicate = await _context.EmployeeTbl.AnyAsync(e => e.Code == dto.Code);
                if (isDuplicate)
                {
                    // Add a model validation error
                    ModelState.AddModelError("Code", "الكود موجود بالفعل سابقا");
                    IEnumerable<Department> departments = await _departmentService.GetAll();
                    ViewData["DepartmentLst"] = new SelectList(departments, "Id", "Name");
                    return View(dto);
                }
                _employeeService.Create(dto);
                TempData["Created"] = "تم الاضافة بنجاح";
                return RedirectToAction("Index");
            }
           
            return View();
        }
        public async Task<IActionResult> Update(int id)
        {
            Employee employee = await _employeeService.GetById(id);
            EmployeeDtoForUpdate data = new EmployeeDtoForUpdate()
            {
                DepartmentId = employee.DepartmentId,
                Id = employee.Id,
                Code = employee.Code,
                Name = employee.Name,
                Address = employee.Address,
                Phone = employee.Phone,
                Gender=employee.Gender,
                BirthOfDate = employee.BirthOfDate,
                CheckInTime = employee.CheckInTime,
                CheckOutTime = employee.CheckOutTime,
                PersonalImageUrl = employee.PersonalImageUrl,
                GraduationCertificateUrl = employee.GraduationCertificateUrl,
                SalaryFormula=employee.SalaryFormula,
                IdentityUrl = employee.IdentityUrl,
                DateOfAppointment = employee.DateOfAppointment,
                ExperienceLevel = employee.ExperienceLevel,
                GrossSalary = employee.GrossSalary,
                Governorate = employee.Governorate,
                JobPosition = employee.JobPosition,
                MaritalStatus = employee.MaritalStatus,
            };


            if (data == null)
            {
                return RedirectToAction("Error", "Home");
            }
            IEnumerable<Department> departments = await _departmentService.GetAll();
            ViewData["DepartmentLst"] = new SelectList(departments, "Id", "Name");
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Update(EmployeeDtoForUpdate dto)
        {
            if (ModelState.IsValid)
            {
                // Check for duplicate code, excluding the code in the dto
                var isDuplicate = await _context.EmployeeTbl
                    .Where(e => e.Code != dto.Code)  // Exclude the code in the dto
                    .AnyAsync(e => e.Code == dto.Code); // Check if any other employee has the same code
                if (isDuplicate)
                {
                    // Add a model validation error
                    ModelState.AddModelError("Code", "الكود موجود بالفعل سابقا");
                    IEnumerable<Department> _departments = await _departmentService.GetAll();
                    ViewData["DepartmentLst"] = new SelectList(_departments, "Id", "Name");
                    return View(dto);
                }
                await _employeeService.Update(dto);
                TempData["Updated"] = "تم التحديث بنجاح";

                return RedirectToAction("Index");
            }
            IEnumerable<Department> departments = await _departmentService.GetAll();
            ViewData["DepartmentLst"] = new SelectList(departments, "Id", "Name");
            return View();
        }
        public async Task<IActionResult> Delete(int id)
        {
          await _employeeService.Delete(id);
            TempData["Deleted"] = "تم الحذف بنجاح";

            return RedirectToAction("Index");
        }
      
        public async Task<IActionResult> Details(int id)
        {
          EmployeeDtoForShow data=  await _employeeService.GetByIdForDetails(id);
            return View(data);
        }
    }
}
