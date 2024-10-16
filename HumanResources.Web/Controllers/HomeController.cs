using HumanResources.Web.Helpers;
using HumanResources.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HumanResources.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            bool globalValue = GlobalVariables.IsAuthenticated;
            if (globalValue)
            {
                ViewBag.IsAuthenticated = globalValue;

                return View();

            }
            else { return RedirectToAction("Login", "Auth"); }
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
