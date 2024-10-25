using HumanResources.Application.DepartmentServices;
using HumanResources.Application.EmployeeServices;
using HumanResources.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static HumanResources.Application.Dtos.EmployeeDto;
using static HumanResources.Domain.Enums.Enums;

namespace HumanResources.Web.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public EmployeeController(IEmployeeService employeeService, IDepartmentService departmentService)
        {
            _employeeService = employeeService;
            _departmentService= departmentService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Employee> data = await _employeeService.GetAll();

            return View(data);
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
                _employeeService.Create(dto);
                TempData["Created"] = "تم الاضافة بنجاح";
                return RedirectToAction("Index");
            }
            IEnumerable<Department> departments = await _departmentService.GetAll();
            ViewData["DepartmentLst"] = new SelectList(departments, "Id", "Name");
            return View();
        }
        public async Task<IActionResult> Update(int id)
        {
            Employee employee = await _employeeService.GetById(id);
            EmployeeDtoForUpdate data = new EmployeeDtoForUpdate()
            {
                DepartmentId = employee.DepartmentId,
                Name = employee.Name,
                Code = employee.Code,
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
