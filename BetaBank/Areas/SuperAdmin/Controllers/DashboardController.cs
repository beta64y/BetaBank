using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.SuperAdmin.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
