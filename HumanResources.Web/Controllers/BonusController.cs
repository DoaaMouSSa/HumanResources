using Microsoft.AspNetCore.Mvc;

namespace HumanResources.Web.Controllers
{
    public class BonusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
