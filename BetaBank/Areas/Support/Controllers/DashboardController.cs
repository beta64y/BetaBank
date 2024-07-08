using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.Support.Controllers
{
    public class DashboardController : Controller
    {
        [Area("Support")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
