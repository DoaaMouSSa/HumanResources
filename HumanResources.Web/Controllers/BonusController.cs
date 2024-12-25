using HumanResources.Application.Dtos;
using HumanResources.Application.EmployeeServices;
using HumanResources.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static HumanResources.Application.Dtos.EmployeeDto;

namespace HumanResources.Web.Controllers
{
    public class BonusController : Controller
    {
        private readonly IEmployeeService _employeeService;
        public BonusController(IEmployeeService employeeService)
        {
            _employeeService=employeeService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            IEnumerable<EmployeeDtoForSelect> employees = await _employeeService.GetAllForSelect();
            ViewData["EmployeeLst"] = new SelectList(employees, "Code", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(EmployeeDtoForAdd dto)
        {

            if (ModelState.IsValid)
            {
               
                 
                TempData["Created"] = "تم الاضافة بنجاح";
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}
