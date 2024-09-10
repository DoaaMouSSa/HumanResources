using HumanResources.Application.DepartmentServices;
using HumanResources.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HumanResources.Web.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            IEnumerable<Department> data = await _departmentService.GetAll(pageNumber, pageSize);
            // Set ViewData for pagination buttons
            ViewData["HasPreviousPage"] = (pageNumber > 1);
            ViewData["HasNextPage"] = (data.Count() == pageSize); // Check if more pages exist
            ViewData["CurrentPage"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            return View(data);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Department dto)
        {
            if (ModelState.IsValid)
            {
                _departmentService.Create(dto);
                TempData["Created"] = "تم الاضافة بنجاح";
                return RedirectToAction("Index");
            }
            return View();
        }
        public async Task<IActionResult> Update(int id)
        {
            Department? data = await _departmentService.GetById(id);
            if(data  == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Update(Department dto)
        {
            if (ModelState.IsValid)
            {
                _departmentService.Update(dto);
                TempData["Updated"] = "تم التحديث بنجاح";

                return RedirectToAction("Index");
            }
            return View();
        }
        public async Task<IActionResult> Delete(int id)
        {
            Department? data = await _departmentService.GetById(id);
            if (data == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Delete(Department dto)
        {

                _departmentService.Delete(dto);
            TempData["Deleted"] = "تم التحديث بنجاح";
            return RedirectToAction("Index");
                    }
    }
}
