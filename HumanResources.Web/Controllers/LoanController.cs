using Microsoft.AspNetCore.Mvc;

namespace HumanResources.Web.Controllers
{
    public class LoanController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
