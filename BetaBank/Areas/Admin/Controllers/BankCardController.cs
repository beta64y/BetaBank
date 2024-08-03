using BetaBank.Areas.Admin.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BankCardController : Controller
    {
        private readonly BetaBankDbContext _context;
        private readonly UserManager<AppUser> _userManager;


        public BankCardController(BetaBankDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<BankCard> bankCards = await _context.BankCards.ToListAsync();

            List<BankCardViewModel> bankCardViewModels = new();
            if (bankCards != null)
            {
                foreach (var bankCard in bankCards)
                {
                    AppUser user = await _context.Users.FirstOrDefaultAsync(x => x.Id == bankCard.UserId);
                    BankCardStatus cardStatus = await _context.BankCardStatuses.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);
                    BankCardType cardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);

                    bankCardViewModels.Add(new BankCardViewModel()
                    {
                        Id = bankCard.Id,
                        CardNumber = bankCard.CardNumber,
                        CVV = bankCard.CVV,
                        ExpiryDate = bankCard.ExpiryDate,
                        Balance = bankCard.Balance,
                        CardStatus = await _context.BankCardStatusModels.FirstOrDefaultAsync(x => x.Id == cardStatus.StatusId),
                        CardType = await _context.BankCardTypeModels.FirstOrDefaultAsync(x => x.Id == cardType.TypeId),
                        UserId = user.Id,
                        UserFirstName = user.FirstName,
                        UserLastName = user.LastName,
                        UserProfilePhoto = user.ProfilePhoto,

                    });
                }
            }
            else
            {
                return NotFound();
            }
            ViewData["UserBankCardViewModels"] = bankCardViewModels;
            TempData["Tab"] = "BankCards";
            return View();
        }
        public async Task<IActionResult> Detail(string id)
        {
            



            BankCard bankCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == id);
            BankCardStatus cardStatus = await _context.BankCardStatuses.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);
            BankCardType cardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);

            UserBankCardViewModel bankCardViewModel = new UserBankCardViewModel()
            {
                        Id = bankCard.Id,
                        CardNumber = bankCard.CardNumber,
                        CVV = bankCard.CVV,
                        ExpiryDate = bankCard.ExpiryDate,
                        Balance = bankCard.Balance,
                        CardStatus = await _context.BankCardStatusModels.FirstOrDefaultAsync(x => x.Id == cardStatus.StatusId),
                        CardType = await _context.BankCardTypeModels.FirstOrDefaultAsync(x => x.Id == cardType.TypeId),

            };

            var user = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == bankCard.UserId);
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
                .Where(x => x.PaidById == bankCard.CardNumber ||x.DestinationId == bankCard.CardNumber )
                .ToList();
            List<Admin.ViewModels.TransactionViewModel> transactionViewModels = new();
            foreach (Transaction transaction in filteredTransactions)
            {
                TransactionTypeModel paidByType = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Id == transaction.PaidByTypeId);
                BankCardType paidByCardType = null;
                if (paidByType.Name == "Card")
                {
                    BankCard card = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == transaction.PaidById);
                    paidByCardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == card.Id);
                }


                TransactionTypeModel destinationType = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Id == transaction.DestinationTypeId);
                BankCardType destinationCardType = null;

                if (destinationType.Name == "Card")
                {
                    BankCard card = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == transaction.DestinationId);
                    destinationCardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == card.Id);

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
            ViewData["Card"] = bankCardViewModel;
            ViewData["User"] = userViewModel;







            return View();
        }
        public async Task<IActionResult> Disable(string id)
        {
            var bankCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == id);
            if (bankCard == null)
            {
                return NotFound();
            }
            BankCardStatus bankCardStatus = await _context.BankCardStatuses.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);
            BankCardStatusModel bankCardStatusModel = await _context.BankCardStatusModels.FirstOrDefaultAsync(x => x.Name == "Disabled");
            bankCardStatus.StatusId = bankCardStatusModel.Id;
            await _context.SaveChangesAsync();

            return Json(new { message = "Card has been Disabled." });
        }
        public async Task<IActionResult> UnDisable(string id)
        {
            var bankCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == id);
            if (bankCard == null)
            {
                return NotFound();
            }
            BankCardStatus bankCardStatus = await _context.BankCardStatuses.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);
            BankCardStatusModel bankCardStatusModel = await _context.BankCardStatusModels.FirstOrDefaultAsync(x => x.Name == "Active");
            bankCardStatus.StatusId = bankCardStatusModel.Id;
            await _context.SaveChangesAsync();

            return Json(new { message = "Card has been UnDisabled." });
        }
    }
}
