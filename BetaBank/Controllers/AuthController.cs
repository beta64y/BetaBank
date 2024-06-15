using BetaBank.Models;
using BetaBank.Utils.Enums;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        //private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AuthController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, SignInManager<AppUser> signInManager)
        {

            //_roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
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
        public async Task<IActionResult> Logout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest();
            }
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel confirmEmailViewModel)
        {
            var user = await _userManager.FindByEmailAsync(confirmEmailViewModel.Email);
            if (user == null)
            {
                return NotFound();
            }
            IdentityResult identityResult = await _userManager.ConfirmEmailAsync(user, confirmEmailViewModel.Token);
            if (!identityResult.Succeeded)
            {
                return BadRequest();//error page                    
            }
            TempData["ConfirmationMessage"] = "Your email Successfully confirmed";

            return RedirectToAction(nameof(Login));
        }
        //public async Task<IActionResult> CreateRole()
        //{
        //    foreach (var items in Enum.GetNames(typeof(Roles)))
        //    {
        //        await _roleManager.CreateAsync(new IdentityRole { Name = items });
        //    }
        //    //await _roleManager.CreateAsync(new IdentityRole { Name = "Admin"});
        //    //await _roleManager.CreateAsync(new IdentityRole { Name = "User" });
        //    //await _roleManager.CreateAsync(new IdentityRole { Name = "Moderator" });
        //    return Content("rollar yarandi");

        //}
    }
}
