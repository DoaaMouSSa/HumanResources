using HumanResources.Application.BonusServices;
using HumanResources.Application.EmployeeServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static HumanResources.Application.Dtos.LoanDto;
using static HumanResources.Application.Dtos.EmployeeDto;
using HumanResources.Application.LoanServices;
using Microsoft.AspNetCore.Authorization;

namespace HumanResources.Web.Controllers
{
    [Authorize]

    public class LoanController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILoanService _loanService;
        public LoanController(IEmployeeService employeeService, ILoanService loanService)
        {
            _employeeService = employeeService;
            _loanService = loanService;
        }
        public async Task<IActionResult> Index()
        {
            var data = await _loanService.GetAll();
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
        public async Task<IActionResult> Create(LoanDtoForAdd dto)
        {

            if (ModelState.IsValid)
            {

                _loanService.Create(dto);

                TempData["Created"] = "تم الاضافة بنجاح";
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}