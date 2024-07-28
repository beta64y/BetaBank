using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.Admin.Controllers
{
    public class CashBackController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
