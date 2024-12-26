using HumanResources.Application.BonusServices;
using HumanResources.Application.Dtos;
using HumanResources.Application.EmployeeServices;
using HumanResources.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static HumanResources.Application.Dtos.BonusDto;
using static HumanResources.Application.Dtos.EmployeeDto;

namespace HumanResources.Web.Controllers
{
    public class BonusController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IBonusService _bonusService;
        public BonusController(IEmployeeService employeeService,IBonusService bonusService)
        {
            _employeeService=employeeService;
            _bonusService = bonusService;
        }
        public async Task<IActionResult> Index()
        {
            var data=await _bonusService.GetAll();
            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            IEnumerable<EmployeeDtoForSelect> employees = await _employeeService.GetAllForSelect();
            ViewData["EmployeeLst"] = new SelectList(employees, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(BonusDtoForAdd dto)
        {

            if (ModelState.IsValid)
            {

                _bonusService.Create(dto);

                TempData["Created"] = "تم الاضافة بنجاح";
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}
