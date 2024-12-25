using HumanResources.Application.EmployeeServices;
using HumanResources.Application.StatesServices;
using HumanResources.Web.Helpers;
using HumanResources.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using static HumanResources.Application.Dtos.EmployeeDto;

namespace HumanResources.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStatesService _satesService;
        public HomeController(ILogger<HomeController> logger, IStatesService satesService)
        {
            _logger = logger;
            _satesService = satesService;
        }

        public async Task<IActionResult> Index()
        {

            var data =await _satesService.GetStates();
                return View(data);

             }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
