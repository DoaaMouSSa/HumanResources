using HumanResources.Application.DepartmentServices;
using HumanResources.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using static HumanResources.Application.Dtos.DepartementDto;

namespace HumanResources.Web.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Department> data = await _departmentService.GetAll();
 
            return View(data);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(DepartmentDtoForAdd dto)
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
        public async Task<IActionResult> Details(int id)
        {
            Department? data = await _departmentService.GetById(id);
            if (data == null)
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
            TempData["Deleted"] = "تم الحذف بنجاح";
            return RedirectToAction("Index");
                    }
    }
}
