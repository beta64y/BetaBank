using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
