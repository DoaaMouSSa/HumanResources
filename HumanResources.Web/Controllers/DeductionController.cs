using HumanResources.Application.BonusServices;
using HumanResources.Application.EmployeeServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static HumanResources.Application.Dtos.LoanDto;
using static HumanResources.Application.Dtos.EmployeeDto;
using HumanResources.Application.LoanServices;
using static HumanResources.Application.Dtos.DeductionDto;

namespace HumanResources.Web.Controllers
{
    public class DeductionController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDeductionService _deductionService;
        public DeductionController(IEmployeeService employeeService, IDeductionService deductionService)
        {
            _employeeService = employeeService;
            _deductionService = deductionService;
        }
        public async Task<IActionResult> Index()
        {
            var data = await _deductionService.GetAll();
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
        public async Task<IActionResult> Create(DeductionDtoForAdd dto)
        {

            if (ModelState.IsValid)
            {

                _deductionService.Create(dto);

                TempData["Created"] = "تم الاضافة بنجاح";
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}