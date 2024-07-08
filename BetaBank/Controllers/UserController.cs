using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Services.Implementations;
using BetaBank.Utils.Enums;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Controllers
{
    public class UserController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly BetaBankDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public UserController(UserManager<AppUser> userManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, BetaBankDbContext context, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser user = await _userManager.Users.FirstOrDefaultAsync(x => x.FIN ==  registerViewModel.FIN);
            if (user != null)
            {
                ModelState.AddModelError("", "FIN must be unique");
                return View(registerViewModel);
            }

            AppUser appUser = new AppUser()
            {
                UserName = registerViewModel.Email,
                FIN = registerViewModel.FIN,
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                DateOfBirth = registerViewModel.DateOfBirth,
                PhoneNumber = registerViewModel.PhoneNumber,
                Email = registerViewModel.Email,
                CreatedDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                IsActive = true,



            };
            IdentityResult identityResult = await _userManager.CreateAsync(appUser, registerViewModel.Password);
            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {

                    ModelState.AddModelError("", error.Description);
                }

                return View();
            }

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

            string link = Url.Action("ConfirmEmail", "Auth", new { email = appUser.Email, token = token },
                HttpContext.Request.Scheme, HttpContext.Request.Host.Value);
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "templates", "ConfirmEmail.html");
            using StreamReader streamReader = new(path);

            string content = await streamReader.ReadToEndAsync();

            string body = content.Replace("[link]", link);

            MailService mailService = new(_configuration);
            await mailService.SendEmailAsync(new MailRequest { ToEmail = appUser.Email, Subject = "Confirm Email", Body = body });

            await _userManager.AddToRoleAsync(appUser, Roles.User.ToString());



            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public async Task<IActionResult> DashBoard()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(user == null)
            {
                return NotFound();
            }
            DashBoardUserViewModel dashBoardUserViewModel = new()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                CreatedDate = user.CreatedDate,
                UpdateDate = user.UpdateDate,
                Email = user.Email

            };



            List<BankCard> bankCards = await _context.BankCards.Where(x => x.UserId == user.Id).ToListAsync();

            List<DashBoardBankCardViewModel> bankCardViewModels = new();
            if (bankCards != null)
            {
                foreach (var bankCard in bankCards)
                {
                    bankCardViewModels.Add(new DashBoardBankCardViewModel()
                    {
                        CardNumber = bankCard.CardNumber,
                        Balance = bankCard.Balance
                    });
                }
            }
            




            DashBoardBankAccountViewModel dashBoardBankAccountViewModel = new();

            BankAccount bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.UserId == user.Id);

            if(bankAccount != null)
            {
                dashBoardBankAccountViewModel = new()
                {
                    AccountNumber = bankAccount.AccountNumber,
                    IBAN = bankAccount.IBAN,
                    Balance = bankAccount.Balance
                };
            }
            


            DashBoardViewModel dashBoardViewModel = new()
            {
                User = dashBoardUserViewModel,
                BankCards = bankCardViewModels,
                BankAccount = dashBoardBankAccountViewModel

            };
            return View(dashBoardViewModel);
        }
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            UserProfileViewmodel userProfileViewModel = new UserProfileViewmodel()
            {
                userUpdateViewModel = new UserUpdateViewModel()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
        }
            };
            return View(userProfileViewModel);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserUpdateViewModel userUpdateViewModel)
        {
            TempData["Tab"] = "account-details";
            UserProfileViewmodel userProfileViewmodel = new()
            {
                userUpdateViewModel = userUpdateViewModel
            };
            if (!ModelState.IsValid)
            {
                return View(nameof(Profile), userProfileViewmodel);
            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            if (user.UserName != userUpdateViewModel.Email && _userManager.Users.Any(u => u.UserName == userUpdateViewModel.Email))
            {
                ModelState.AddModelError("UserName", "UserName Must be unique");
                return View(nameof(Profile), userProfileViewmodel);
            }
            if (user.Email != userUpdateViewModel.Email && _userManager.Users.Any(u => u.Email == userUpdateViewModel.Email))
            {
                ModelState.AddModelError("Email", "Email Must be unique");
                return View(nameof(Profile), userProfileViewmodel);
            }

            if (userUpdateViewModel.CurrentPassword != null)
            {
                if (userUpdateViewModel.NewPassword == null)
                {
                    ModelState.AddModelError("NewPassword", "new password bos ola bilmez");
                    return View(nameof(Profile), userProfileViewmodel);

                }
                IdentityResult identityResult = await _userManager.ChangePasswordAsync(user, userUpdateViewModel.CurrentPassword, userUpdateViewModel.NewPassword);
                if (!identityResult.Succeeded)
                {
                    foreach (var i in identityResult.Errors)
                    {
                        ModelState.AddModelError("", i.Description);
                    }
                    return View(nameof(Profile), userProfileViewmodel);
                }
            }

            user.FirstName = userUpdateViewModel.FirstName;
            user.LastName = userUpdateViewModel.LastName;
            user.UserName = userUpdateViewModel.Email;
            user.Email = userUpdateViewModel.Email;
            user.UpdateDate = DateTime.UtcNow;

            IdentityResult result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var i in result.Errors)
                {
                    ModelState.AddModelError("", i.Description);
                }
                return View(nameof(Profile), userProfileViewmodel);

            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Sizin profiliniz ugurla yenilendi";
            return View(nameof(Profile), userProfileViewmodel);


        }


    }
}
