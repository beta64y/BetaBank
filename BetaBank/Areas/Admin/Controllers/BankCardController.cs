using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.Admin.Controllers
{
    public class BankCardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
