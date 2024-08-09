using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Services.Implementations;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Controllers
{
    public class BankAccountAndWalletController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly BetaBankDbContext _context;

        public BankAccountAndWalletController(UserManager<AppUser> userManager, BetaBankDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        public async Task<IActionResult> Details()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }


            BankAccountDetailsViewModel bankAccountViewModel = null;

            BankAccount bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.UserId == user.Id);

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
            ViewData["BankAccount"] = bankAccountViewModel;




            CashBack cashBack = await _context.CashBacks.FirstOrDefaultAsync(x => x.UserId == user.Id);
            CashBackDetailsViewModel wallet = new()
            {
                Id = cashBack.Id,
                Balance = cashBack.Balance,
                CreatedDate = cashBack.CreatedDate,
                UpdatedDate = cashBack.UpdatedDate,
                CashBackNumber = cashBack.CashBackNumber,
            };

            ViewData["CashBack"] = wallet;

            //start Transaction
            List<Transaction> allTransactions = await _context.Transactions
.AsNoTracking().Where(x =>
                    x.PaidById == cashBack.CashBackNumber ||
                    x.PaidById == bankAccount.AccountNumber ||
                    x.DestinationId == bankAccount.AccountNumber
                ).ToListAsync();


           

            List<TransactionDetailsViewModel> transactionViewModels = new();

            foreach (Transaction transaction in allTransactions.OrderByDescending(x => x.TransactionDate))
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
                if (
                    transaction.PaidById == cashBack.CashBackNumber ||
                    transaction.PaidById == bankAccount.AccountNumber
                )
                {
                    summary = "Expense";

                }
                else if (
                    
                    transaction.DestinationId == bankAccount.AccountNumber
                )
                {
                    summary = "Income";

                }

                transactionViewModels.Add(new TransactionDetailsViewModel()
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
                    Summary = summary,


                });

            }


            // Get the last 5 months
            List<string> last5Months = StatisticsService.GetLastMonths(5);

            // Calculate the start date for filtering transactions
            DateTime startDate = DateTime.Now.AddMonths(-4).AddDays(-DateTime.Now.Day + 1);

            // Calculate income data for the last 5 months
            List<double> incomeData = last5Months.Select(month =>
            {
                return transactionViewModels
                    .Where(t => t.Summary == "Income" && t.TransactionDate >= startDate && t.TransactionDate.Month == DateTime.ParseExact(month, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month)
                    .Sum(t => t.Amount);
            }).ToList();

            // Calculate income and expense data for the current and previous months
            DateTime now = DateTime.Now;
            DateTime startCurrentMonth = new DateTime(now.Year, now.Month, 1);
            DateTime startPreviousMonth = startCurrentMonth.AddMonths(-1);

            double currentMonthIncome = transactionViewModels
                .Where(t => t.Summary == "Income" && t.TransactionDate >= startCurrentMonth && t.TransactionDate < now)
                .Sum(t => t.Amount);

            double currentMonthExpense = transactionViewModels
                .Where(t => t.Summary == "Expense" && t.TransactionDate >= startCurrentMonth && t.TransactionDate < now)
                .Sum(t => t.Amount);

            double previousMonthIncome = transactionViewModels
                .Where(t => t.Summary == "Income" && t.TransactionDate >= startPreviousMonth && t.TransactionDate < startCurrentMonth)
                .Sum(t => t.Amount);

            double previousMonthExpense = transactionViewModels
                .Where(t => t.Summary == "Expense" && t.TransactionDate >= startPreviousMonth && t.TransactionDate < startCurrentMonth)
                .Sum(t => t.Amount);

            MonthlyIncomeExpenseViewModel monthlyIncomeExpense = new()
            {
                PreviousMonthExpense = previousMonthExpense,
                PreviousMonthIncome = previousMonthIncome,
                CurrentMonthExpense = currentMonthExpense,
                CurrentMonthIncome = currentMonthIncome
            };

            // Store data in ViewData
            ViewData["Last5Months"] = last5Months;
            ViewData["IncomeData"] = incomeData;
            ViewData["MonthlyIncomeExpense"] = monthlyIncomeExpense;

            ViewData["Transactions"] = transactionViewModels;






            return View();
        }

    }
}
