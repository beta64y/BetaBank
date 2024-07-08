using BetaBank.Models;
using BetaBank.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetaBank.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;


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
            List<AppUser> users = await _userManager.Users
                .AsNoTracking()
                .OrderBy(b => b.CreatedDate)
                .ToListAsync();


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
            });
            }

            return View(usersViewModel);
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
                        CardNumber = bankCard.CardNumber,
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
    }
}
