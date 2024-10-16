using HumanResources.Application.DepartmentServices;
using HumanResources.Domain.Entities;
using HumanResources.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
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
            bool globalValue = GlobalVariables.IsAuthenticated;
            if (globalValue)
            {
                ViewBag.IsAuthenticated = globalValue;

                IEnumerable<Department> data = await _departmentService.GetAll();

                return View(data);
            }
            else { return RedirectToAction("Login", "Auth"); }
          
        }
        [HttpGet]
        public IActionResult Create()
        {
            bool globalValue = GlobalVariables.IsAuthenticated;
            if (globalValue)
            {
                ViewBag.IsAuthenticated = globalValue;
                return View();

            }
            else { return RedirectToAction("Login", "Auth"); }
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
            bool globalValue = GlobalVariables.IsAuthenticated;
            if (globalValue)
            {
                Department? data = await _departmentService.GetById(id);
                if (data == null)
                {
                    return RedirectToAction("Error", "Home");
                }
                return View(data);

            }
            else { return RedirectToAction("Login", "Auth"); }
           
        }
        public async Task<IActionResult> Details(int id)
        {
            bool globalValue = GlobalVariables.IsAuthenticated;
            if (globalValue)
            {
                Department? data = await _departmentService.GetById(id);
                if (data == null)
                {
                    return RedirectToAction("Error", "Home");
                }
                return View(data);
            }
            else { return RedirectToAction("Login", "Auth"); }
         
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
            bool globalValue = GlobalVariables.IsAuthenticated;
            if (globalValue)
            {
                Department? data = await _departmentService.GetById(id);
                if (data == null)
                {
                    return RedirectToAction("Error", "Home");
                }
                return View(data);
            }
            else { return RedirectToAction("Login", "Auth"); }
       
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
