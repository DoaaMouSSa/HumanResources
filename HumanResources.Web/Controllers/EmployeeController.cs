using HumanResources.Application.DepartmentServices;
using HumanResources.Application.EmployeeServices;
using HumanResources.Domain.Entities;
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
        public IActionResult Create(EmployeeDtoForAdd dto)
        {
            if (ModelState.IsValid)
            {
                _employeeService.Create(dto);
                TempData["Created"] = "تم الاضافة بنجاح";
                return RedirectToAction("Index");
            }
            return View();
        }
        public async Task<IActionResult> Update(int id)
        {
            Employee? data = await _employeeService.GetById(id);
            if (data == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Update(Employee dto)
        {
            if (ModelState.IsValid)
            {
                _employeeService.Update(dto);
                TempData["Updated"] = "تم التحديث بنجاح";

                return RedirectToAction("Index");
            }
            return View();
        }
        public async Task<IActionResult> Delete(int id)
        {
            Employee? data = await _employeeService.GetById(id);
            if (data == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Delete(Employee dto)
        {
            _employeeService.Delete(dto);
            TempData["Deleted"] = "تم الحذف بنجاح";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Details(int id)
        {
            //EmployeeDtoForShow data = await _departmentService.GetById(id);
            //if (data == null)
            //{
            //    return RedirectToAction("Error", "Home");
            //}
            //return View(data);
            throw new Exception();
        }
    }
}
