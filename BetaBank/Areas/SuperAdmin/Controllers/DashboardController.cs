using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
    }
}
