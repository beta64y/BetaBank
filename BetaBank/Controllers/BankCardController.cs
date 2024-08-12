using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.ViewModels;
using BetaBank.Services.Implementations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using BetaBank.Utils.Enums;

namespace BetaBank.Controllers
{
    
    public class BankCardController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly BetaBankDbContext _context;

        public BankCardController(BetaBankDbContext context, UserManager<AppUser> userManager)
        {

            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Cards()
        {
            ViewData["Types"] = await _context.BankCardTypeModels.AsNoTracking().ToListAsync();
            

            return View();
        }
        public async Task<IActionResult> GetCard(string id)
        {
            if(!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }
            ViewData["Type"] = await _context.BankCardTypeModels.FirstOrDefaultAsync(x => x.Id == id);
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetCard(GetBankCardViewModel getBankCardViewModel , string id)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Type"] = await _context.BankCardTypeModels.FirstOrDefaultAsync(x => x.Id == id);
                return View();
            }
            if (!getBankCardViewModel.IsTermsAndConditionsAccepted)
            {
                ViewData["Type"] = await _context.BankCardTypeModels.FirstOrDefaultAsync(x => x.Id == id);
                ModelState.AddModelError("", "You must accept the terms and conditions to proceed.");
                return View();
            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            if (await _context.BankCards.Where(x => x.UserId == user.Id).CountAsync() >= 3)
            {
                ViewBag.Message = "Maximum card count reached!";
                return View("Warning");
            }
            string cardNumber;
            bool isUnique;

            do
            {
                cardNumber = BankCardService.GenerateCardNumber();
                isUnique = !await _context.BankCards.AnyAsync(bc => bc.CardNumber == cardNumber);
            } while (!isUnique);

            var bankCard = new BankCard()
            {
                Id = $"{Guid.NewGuid()}",
                CardNumber = cardNumber,
                CVV = BankCardService.GenerateCVV(),
                CreatedDate = DateTime.UtcNow,
                ExpiryDate = BankCardService.GenerateExpiryDate(),
                Balance = 0,
                UserId = user.Id,
            };

            



            var status = await _context.BankCardStatusModels.FirstOrDefaultAsync(x => x.Name == "Active");
            if(status == null)
            {
                return NotFound();
            }
            var bankCardStatus = new Models.BankCardStatus()
            {
                Id = $"{Guid.NewGuid()}",
                CardId = bankCard.Id,
                StatusId = status.Id

            };

            var type = await _context.BankCardTypeModels.FirstOrDefaultAsync(x => x.Id == id);
            if (type == null)
            {
                return NotFound();
            }
            var bankCardType = new Models.BankCardType()
            {
                Id = $"{Guid.NewGuid()}",
                CardId = bankCard.Id,
                TypeId = type.Id
            };


            await _context.BankCards.AddAsync(bankCard);
            await _context.BankCardStatuses.AddAsync(bankCardStatus);
            await _context.BankCardTypes.AddAsync(bankCardType);
            await _context.SaveChangesAsync();

            return  RedirectToAction("Index", "Home");
        }
        
        [Authorize]
        public async Task<IActionResult> Details(string id)
        {
            BankCard bankCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == id);
            
            var user = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == bankCard.UserId);
            if (user == null)
            {
                return NotFound();
            }

            Models.BankCardStatus cardStatus = await _context.BankCardStatuses.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);
            Models.BankCardType cardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);

            BankCardDetailsViewModel bankCardViewModel = new BankCardDetailsViewModel()
            {
                Id = bankCard.Id,
                CardNumber = bankCard.CardNumber,
                CVV = bankCard.CVV,
                ExpiryDate = bankCard.ExpiryDate,
                Balance = bankCard.Balance,
                CardStatus = await _context.BankCardStatusModels.FirstOrDefaultAsync(x => x.Id == cardStatus.StatusId),
                CardType = await _context.BankCardTypeModels.FirstOrDefaultAsync(x => x.Id == cardType.TypeId),
                UserFirstName = user.FirstName,
                UserLastName = user.LastName,

            };

            
            List<Transaction> filteredTransactions = await _context.Transactions.AsNoTracking().OrderByDescending(x => x.TransactionDate).Where(x => x.PaidById == bankCard.CardNumber || x.DestinationId == bankCard.CardNumber).ToListAsync();

          
            List<TransactionDetailsViewModel> transactionViewModels = new();
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
                string summary = null;
                
                if ( transaction.PaidById == bankCardViewModel.CardNumber)
                {
                    summary = "Expense";

                }
                else if (transaction.DestinationId == bankCardViewModel.CardNumber )
                {
                    summary = "Income";

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
                    Summary=  summary

                });

            }

            List<string> lastMonths = StatisticsService.GetLastMonths(5);
            DateTime startDate = DateTime.Now.AddMonths(-4).AddDays(-DateTime.Now.Day + 1);

            List<double> incomeData = lastMonths.Select(month =>
            {
                return transactionViewModels
                    .Where(t => t.Summary == "Income" && t.TransactionDate >= startDate && t.TransactionDate.Month == DateTime.ParseExact(month, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month)
                    .Sum(t => t.Amount);
            }).ToList();

            ViewData["LastMonths"] = lastMonths;
            ViewData["IncomeData"] = incomeData;
            //ViewData["LastMonths"] = StatisticsService.GetLastMonths(5);
            ViewData["Transactions"] = transactionViewModels;
            ViewData["Card"] = bankCardViewModel;



            return View();
        }
        public async Task<IActionResult> Block(string id)
        {
            var bankCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == id);
            if (bankCard == null)
            {
                return NotFound();
            }
            Models.BankCardStatus bankCardStatus = await _context.BankCardStatuses.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);
            BankCardStatusModel disabledStatus = await _context.BankCardStatusModels.FirstOrDefaultAsync(x => x.Name == "Disabled");
            if (disabledStatus.Id == bankCardStatus.StatusId) 
            {
                ViewBag.Message = "Something went wrong!";
                return View("Error");
            }
            else
            {
            BankCardStatusModel bankCardStatusModel = await _context.BankCardStatusModels.FirstOrDefaultAsync(x => x.Name == "Blocked");
            bankCardStatus.StatusId = bankCardStatusModel.Id;
  
            await _context.SaveChangesAsync();


            return Json(new { message = "Card has been Blocked." });
            }
            
        }
        public async Task<IActionResult> UnBlock(string id)
        {
            var bankCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == id);
            if (bankCard == null)
            {
                return NotFound();
            }
            Models.BankCardStatus bankCardStatus = await _context.BankCardStatuses.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);
            BankCardStatusModel disabledStatus = await _context.BankCardStatusModels.FirstOrDefaultAsync(x => x.Name == "Disabled");

            if (disabledStatus.Id == bankCardStatus.StatusId)
            {
                ViewBag.Message = "Something went wrong!";
                return View("Error");
            }
            else
            {
            BankCardStatusModel bankCardStatusModel = await _context.BankCardStatusModels.FirstOrDefaultAsync(x => x.Name == "Active");
            bankCardStatus.StatusId = bankCardStatusModel.Id;
            
            await _context.SaveChangesAsync();


            return Json(new { message = "Card has been UnBlocked." });
            }
            
        }






    }
}
