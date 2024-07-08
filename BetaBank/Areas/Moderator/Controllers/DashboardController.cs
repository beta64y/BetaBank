using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.Moderator.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
