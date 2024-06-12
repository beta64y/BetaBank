using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            return View();
        }
    }
}
