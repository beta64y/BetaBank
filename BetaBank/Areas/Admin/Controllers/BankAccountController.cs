using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.Admin.Controllers
{
    public class BankAccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
