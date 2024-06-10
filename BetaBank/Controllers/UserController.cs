using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }
    }
}
