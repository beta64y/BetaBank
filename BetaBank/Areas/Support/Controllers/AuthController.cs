using BetaBank.Models;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.Support.Controllers
{
    [Area("Support")]
    public class AuthController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, SignInManager<AppUser> signInManager)
        {

            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {
                Console.WriteLine(ModelState.ErrorCount);
                ModelState.AddModelError("", "");
                return View();
            }
            var user = await _userManager.FindByNameAsync(loginViewModel.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email or Password is incorrect");
                return View();
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "please confirm mail");
                return View();
            }
            if (user.Banned)
            {
                ModelState.AddModelError("", "Banlandiniz");
                return View();
            }
            var signInResult = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, true);
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Get sonra gelersen");
                return View();
            }
            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password is incorrect");
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
