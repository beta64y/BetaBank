using BetaBank.Models;
using BetaBank.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetaBank.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using BetaBank.Services.Implementations;


namespace BetaBank.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly BetaBankDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public UserController(UserManager<AppUser> userManager, BetaBankDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync("User");

            var users = usersInRole
                .AsQueryable()
                .AsNoTracking()
                .OrderBy(b => b.CreatedDate)
                .ToList();


            List<UserViewModel> usersViewModel = new List<UserViewModel>();
            foreach (var user in users)
            {
                usersViewModel.Add(new UserViewModel
                {
                     Id = user.Id,
                     FirstName = user.FirstName,
                     LastName = user.LastName,
                     DateOfBirth = user.DateOfBirth,
                     PhoneNumber = user.PhoneNumber,
                     CreatedDate = user.CreatedDate,
                     UpdateDate = user.UpdateDate,
                     Banned = user.Banned,
                     ProfilePhoto = user.ProfilePhoto,
                     Email= user.Email,
                     Age = user.DateOfBirth.CalculateAge(),
                    EmailConfirmed = user.EmailConfirmed ,
                });
            }

            AdminUserViewModel ViewModel = new()
            {
                Users = usersViewModel,
            };
            TempData["Tab"] = "Users";
            return View(ViewModel);
        }
        public async Task<IActionResult> BanUser(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            user.Banned = true;
            await _context.SaveChangesAsync();

            return Json(new { message = "User has been Banned." });
        }
        public async Task<IActionResult> UnBanUser(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            user.Banned = false;
            await _context.SaveChangesAsync();

            return Json(new { message = "User has been UnBanned." });
        }
        public async Task<IActionResult> Detail(string id)
        {
            var user = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if(user == null)
            {
                return NotFound();
            }

            UserViewModel userViewModel = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
                CreatedDate = user.CreatedDate,
                UpdateDate = user.UpdateDate,
                Banned = user.Banned,
                ProfilePhoto = user.ProfilePhoto,
                Email = user.Email,
                Age = user.DateOfBirth.CalculateAge(),
                EmailConfirmed = user.EmailConfirmed,
            };



            List<BankCard> bankCards = await _context.BankCards.Where(x => x.UserId == user.Id).ToListAsync();

            List<UserBankCardViewModel> bankCardViewModels = new();
            if (bankCards != null)
            {
                foreach (var bankCard in bankCards)
                {
                    BankCardStatus cardStatus = await _context.BankCardStatuses.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);
                    BankCardType cardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);

                    bankCardViewModels.Add(new UserBankCardViewModel()
                    {
                        Id = bankCard.Id,
                        CardNumber = bankCard.CardNumber.ToCreditCardFormat(),
                        CVV = bankCard.CVV, 
                        ExpiryDate = bankCard.ExpiryDate,
                        Balance = bankCard.Balance,
                        CardStatus = await _context.BankCardStatusModels.FirstOrDefaultAsync(x => x.Id == cardStatus.StatusId),
                        CardType = await _context.BankCardTypeModels.FirstOrDefaultAsync(x => x.Id == cardType.TypeId),

                    });
                }
            }





            UserBankAccountViewModel bankAccountViewModel = null;

            BankAccount bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (bankAccount != null)
            {
                BankAccountStatus accountStatus = await _context.BankAccountStatuses.FirstOrDefaultAsync(x => x.AccountId == bankAccount.Id);
                bankAccountViewModel = new()
                {
                    Id = bankAccount.Id,
                    AccountNumber = bankAccount.AccountNumber,
                    IBAN = bankAccount.IBAN,
                    Balance = bankAccount.Balance,
                    AccountStatus = await _context.BankAccountStatusModels.FirstOrDefaultAsync(x => x.Id == accountStatus.StatusId),

                };
            }





            UserDetailViewModel userDetailViewModel = new()
            {
                User = userViewModel,
                Account = bankAccountViewModel,
                Cards = bankCardViewModels,
                


            };



            return View(userDetailViewModel);
        }
        public async Task<IActionResult> Search(AdminUserViewModel adminUsersViewModel)
        {
            if (adminUsersViewModel.Search.SearchTerm != null)
            {
                var searchTerm = adminUsersViewModel.Search.SearchTerm.ToLower();
                var filteredUsers = await _context.Users.Where(p => p.FirstName.ToLower().Contains(searchTerm) || p.LastName.ToLower().Contains(searchTerm) || p.Email.ToLower().Contains(searchTerm) || p.PhoneNumber.ToLower().Contains(searchTerm)  ).ToListAsync();
                List<UserViewModel> filteredUsersModel = new();
                foreach (var user in filteredUsers)
                {
                    filteredUsersModel.Add(new UserViewModel
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        DateOfBirth = user.DateOfBirth,
                        PhoneNumber = user.PhoneNumber,
                        CreatedDate = user.CreatedDate,
                        UpdateDate = user.UpdateDate,
                        Banned = user.Banned,
                        ProfilePhoto = user.ProfilePhoto,
                        Email = user.Email,
                        Age = user.DateOfBirth.CalculateAge(),
                        EmailConfirmed = user.EmailConfirmed,
                    });
                }
                AdminUserViewModel ViewModel = new AdminUserViewModel()
                {
                    Users = filteredUsersModel,
                    Search = adminUsersViewModel.Search
                };
                TempData["Tab"] = "Subscribers";
                return View("Index", ViewModel);
            }
            else
            {
                TempData["Tab"] = "Subscribers";
                return View(null);
            }
        }
    }
}
