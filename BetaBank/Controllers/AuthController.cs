using BetaBank.Models;
using BetaBank.Services.Implementations;
using BetaBank.Utils.Enums;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AuthController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, SignInManager<AppUser> signInManager)
        {

            _roleManager = roleManager;
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
            var userRoles = await _userManager.GetRolesAsync(user);
            if (user == null || !userRoles.Contains("User"))
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
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(forgotPasswordViewModel.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email Not Found");
                return View();
            }
            //https://localhost:7176/Auth/Reset?email=&token=
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            //string link = Url.Action("ResetPassword", "Auth", new { email = user.Email, token = token });
            string link = Url.Action("ResetPassword", "Auth", new { email = user.Email, token = token },
            HttpContext.Request.Scheme, HttpContext.Request.Host.Value);
            
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "templates", "ResetPassword.html");
            using StreamReader streamReader = new(path);

            string content = await streamReader.ReadToEndAsync();

            string body = content.Replace("[link]", link);

            MailService mailService = new(_configuration);
            await mailService.SendEmailAsync(new MailRequest { ToEmail = user.Email, Subject = "Reset Password", Body = body });
            return RedirectToAction(nameof(Login));
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

        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
            if (user == null)
            {
                return NotFound();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(SubmitResetPasswordViewModel submitResetPasswordViewModel, string email, string token)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            IdentityResult identityResult = await _userManager.ResetPasswordAsync(user, token, submitResetPasswordViewModel.Password);
            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
            return RedirectToAction(nameof(Login));

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
        public async Task<IActionResult> CreateRole()
        {
            foreach (var items in Enum.GetNames(typeof(Roles)))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = items });
            }
            //await _roleManager.CreateAsync(new IdentityRole { Name = "Admin"});
            //await _roleManager.CreateAsync(new IdentityRole { Name = "User" });
            //await _roleManager.CreateAsync(new IdentityRole { Name = "Moderator" });
            return Content("rollar yarandi");

        }
    }
}
