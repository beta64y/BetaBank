using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
