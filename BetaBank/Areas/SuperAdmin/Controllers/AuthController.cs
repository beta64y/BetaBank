﻿using BetaBank.Models;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    public class AuthController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;



        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {

            _userManager = userManager;
            _signInManager = signInManager;
  
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
                ModelState.AddModelError("", "");
                return View();
            }
            var user = await _userManager.FindByNameAsync(loginViewModel.UsernameOrEmail);
            if (user == null )
            {
                ModelState.AddModelError("", "Email or Password is incorrect!");
                return View();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Contains("SuperAdmin"))
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
            return RedirectToAction("Index", "DashBoard");
        }
        public async Task<IActionResult> Logout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest();
            }
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }
    }
}
