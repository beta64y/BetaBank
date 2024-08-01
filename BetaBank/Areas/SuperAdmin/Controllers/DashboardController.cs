using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            TempData["Tab"] = "Dashboard";
            return View();

        }
    }
}
