using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Utils.Enums;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    public class AuthController : Controller
    {
        private readonly BetaBankDbContext _context;

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;



        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, BetaBankDbContext context)
        {

            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            if (!ModelState.IsValid)
            {
                Console.WriteLine(ModelState.ErrorCount);
                ModelState.AddModelError("", "");
                return View();
            }
            var user = await _userManager.FindByNameAsync(loginViewModel.UsernameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError("", "Email or Password is incorrect!");
                return View();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Contains("Moderator"))
            {
                ModelState.AddModelError("", "Email or Password is incorrect!");
                return View();
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Please confirm Email!");
                return View();
            }
            if (user.Banned)
            {
                ModelState.AddModelError("", "Your account has been banned. Please contact support for more information.");
                return View();
            }
            var signInResult = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, true);
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Your account has been locked out. Please try again later.");
                return View();
            }
            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password is incorrect!");
                return View();
            }
            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = user.Id,
                Action = UserActionType.Logined.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.Auth.ToString(),
                EntityType = EntityType.None.ToString(),
            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard");
        }
        public async Task<IActionResult> Logout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest();
            }
            await _signInManager.SignOutAsync();
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = user.Id,
                Action = UserActionType.Logouted.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.Auth.ToString(),
                EntityType = EntityType.None.ToString(),
            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login", "Auth");
        }
    }
}
