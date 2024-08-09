using BetaBank.Areas.Admin.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Services.Implementations;
using BetaBank.Utils.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BankAccountController : Controller
    {
        private readonly BetaBankDbContext _context;
        private readonly UserManager<AppUser> _userManager;


        public BankAccountController(BetaBankDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            List<BankAccount> bankAccounts = await _context.BankAccounts.AsNoTracking().ToListAsync();
            List<Admin.ViewModels.BankAccountViewModel> bankAccountViewModels = new();
            foreach (BankAccount bankAccount in bankAccounts)
            {
                AppUser user = await _context.Users.FirstOrDefaultAsync(x => x.Id == bankAccount.UserId);
                Models.BankAccountStatus accountStatus = await _context.BankAccountStatuses.FirstOrDefaultAsync(x => x.AccountId == bankAccount.Id);

                bankAccountViewModels.Add(new Admin.ViewModels.BankAccountViewModel()
                {
                    Id = bankAccount.Id,
                    AccountStatus = await _context.BankAccountStatusModels.FirstOrDefaultAsync(x => x.Id == accountStatus.StatusId),
                    AccountNumber = bankAccount.AccountNumber,
                    Balance = bankAccount.Balance,
                    IBAN = bankAccount.IBAN,
                    UserId = user.Id,
                    UserFirstName = user.FirstName,
                    UserLastName = user.LastName,
                    UserProfilePhoto = user.ProfilePhoto,

                });
            }
            ViewData["BankAccounts"] = bankAccountViewModels;




            var employee = await _userManager.FindByNameAsync(User.Identity.Name);
            if (employee == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = employee.Id,
                Action = UserActionType.Get.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.Accounts.ToString(),
                EntityType = EntityType.Page.ToString(),
                EntityId = "Index"
            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();




            TempData["Tab"] = "BankAccounts";
            return View();
        }
        public async Task<IActionResult> Detail(string id)
        {




            UserBankAccountViewModel bankAccountViewModel = null;

            BankAccount bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.AccountNumber == id);

            if (bankAccount != null)
            {
                Models.BankAccountStatus accountStatus = await _context.BankAccountStatuses.FirstOrDefaultAsync(x => x.AccountId == bankAccount.Id);
                bankAccountViewModel = new()
                {
                    Id = bankAccount.Id,
                    AccountNumber = bankAccount.AccountNumber,
                    IBAN = bankAccount.IBAN,
                    Balance = bankAccount.Balance,
                    AccountStatus = await _context.BankAccountStatusModels.FirstOrDefaultAsync(x => x.Id == accountStatus.StatusId),

                };
            }
            else
            {
                return NotFound();
            }

            var user = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == bankAccount.UserId);
            if (user == null)
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



            List<Transaction> allTransactions = await _context.Transactions
    .AsNoTracking()
    .ToListAsync();

            List<Transaction> filteredTransactions = allTransactions
                .Where(x => x.PaidById == bankAccount.AccountNumber || x.DestinationId == bankAccount.AccountNumber)
                .ToList();
            List<Admin.ViewModels.TransactionViewModel> transactionViewModels = new();
            foreach (Transaction transaction in filteredTransactions)
            {
                TransactionTypeModel paidByType = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Id == transaction.PaidByTypeId);
                Models.BankCardType paidByCardType = null;
                if (paidByType.Name == "Card")
                {
                    BankCard card = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == transaction.PaidById);
                    paidByCardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == card.Id);
                }


                TransactionTypeModel destinationType = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Id == transaction.DestinationTypeId);
                Models.BankCardType destinationCardType = null;

                if (destinationType.Name == "Card")
                {
                    BankCard card = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == transaction.DestinationId);
                    if (card != null)
                    {
                        destinationCardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == card.Id);
                    }



                }


                transactionViewModels.Add(new()
                {
                    Id = transaction.Id,
                    ReceiptNumber = transaction.ReceiptNumber,
                    Amount = transaction.Amount,
                    Commission = transaction.Commission,
                    BillingAmount = transaction.BillingAmount,
                    CashbackAmount = transaction.CashbackAmount,
                    TransactionDate = transaction.TransactionDate,
                    PaidByType = paidByType,
                    PaidById = transaction.PaidById,
                    DestinationType = destinationType,
                    DestinationId = transaction.DestinationId,
                    Status = await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Id == transaction.StatusId),
                    Title = transaction.Title,
                    Description = transaction.Description,
                    PaidByCardType = paidByCardType != null ? await _context.BankCardTypeModels.FirstOrDefaultAsync(x => x.Id == paidByCardType.TypeId) : null,
                    DestinationCardType = destinationCardType != null ? await _context.BankCardTypeModels.FirstOrDefaultAsync(x => x.Id == destinationCardType.TypeId) : null,


                });

            }



            ViewData["Transactions"] = transactionViewModels;
            ViewData["Account"] = bankAccountViewModel;
            ViewData["User"] = userViewModel;





            var employee = await _userManager.FindByNameAsync(User.Identity.Name);
            if (employee == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = employee.Id,
                Action = UserActionType.Viewed.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.Accounts.ToString(),
                EntityType = EntityType.BankAccount.ToString(),
                EntityId = bankAccount.AccountNumber
            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();










            return View();
        }

        public async Task<IActionResult> Suspend(string id)
        {
            BankAccount bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.AccountNumber == id);
            if (bankAccount == null)
            {
                return NotFound();
            }
            Models.BankAccountStatus bankAccountStatus = await _context.BankAccountStatuses.FirstOrDefaultAsync(x => x.AccountId == bankAccount.Id);
            BankAccountStatusModel bankAccountStatusModel = await _context.BankAccountStatusModels.FirstOrDefaultAsync(x => x.Name == "Suspended");
            bankAccountStatus.StatusId = bankAccountStatusModel.Id;

            var employee = await _userManager.FindByNameAsync(User.Identity.Name);
            if (employee == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = employee.Id,
                Action = UserActionType.Suspended.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.Accounts.ToString(),
                EntityType = EntityType.BankAccount.ToString(),
                EntityId = bankAccount.AccountNumber,
            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();


            return Json(new { message = "Account has been Suspended." });
        }
        public async Task<IActionResult> UnSuspend(string id)
        {
            BankAccount bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.AccountNumber == id);
            if (bankAccount == null)
            {
                return NotFound();
            }
            Models.BankAccountStatus bankAccountStatus = await _context.BankAccountStatuses.FirstOrDefaultAsync(x => x.AccountId == bankAccount.Id);
            BankAccountStatusModel bankAccountStatusModel = await _context.BankAccountStatusModels.FirstOrDefaultAsync(x => x.Name == "Active");
            bankAccountStatus.StatusId = bankAccountStatusModel.Id;

            var employee = await _userManager.FindByNameAsync(User.Identity.Name);
            if (employee == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = employee.Id,
                Action = UserActionType.MakeActive.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.Accounts.ToString(),
                EntityType = EntityType.BankAccount.ToString(),
                EntityId = bankAccount.AccountNumber,
            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();


            return Json(new { message = "Account has been Active." });
        }
    }
}
