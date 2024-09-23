using HumanResources.Application.EmployeeServices;
using HumanResources.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HumanResources.Web.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Employee> data = await _employeeService.GetAll();

            return View(data);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Employee dto)
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
    }
}
