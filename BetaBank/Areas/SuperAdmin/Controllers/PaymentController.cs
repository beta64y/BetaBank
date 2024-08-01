using BetaBank.Areas.Admin.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Services.Implementations;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace BetaBank.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    public class PaymentController : Controller
    {
        private readonly BetaBankDbContext _context;

        public PaymentController(BetaBankDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Transaction> transactions = await _context.Transactions.AsNoTracking().ToListAsync();
            List<Admin.ViewModels.TransactionViewModel> transactionViewModels = new();
            foreach (Transaction transaction in transactions)
            {
                TransactionTypeModel paidByType = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Id == transaction.PaidByTypeId);
                BankCardType paidByCardType = null;
                if (paidByType.Name == "Card")
                {
                    BankCard card = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == transaction.PaidById);
                    paidByCardType = await _context.BankCardTypes.FirstOrDefaultAsync(x =>x.CardId == card.Id);
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
            
            TempData["Tab"] = "Payments";
            return View();
        }



        public async Task<IActionResult> Detail(string id)
        {
            Transaction transaction = await _context.Transactions.FirstOrDefaultAsync(x => x.Id == id);
            if (transaction == null)
            {
                return BadRequest();
            }
            Admin.ViewModels.TransactionViewModel transactionViewModel = new()
            {
                Id = transaction.Id,
                ReceiptNumber = transaction.ReceiptNumber,
                Amount = transaction.Amount,
                Commission = transaction.Commission,
                BillingAmount = transaction.BillingAmount,
                CashbackAmount = transaction.CashbackAmount,
                TransactionDate = transaction.TransactionDate,
                PaidByType = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Id == transaction.PaidByTypeId),
                PaidById = transaction.PaidById,
                DestinationType = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Id == transaction.DestinationTypeId),
                DestinationId = transaction.DestinationId,
                Status = await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Id == transaction.StatusId),
                Title = transaction.Title,
                Description = transaction.Description
            };



            //paidby

            PaymentDetailsViewModel paidBy = new();

            UserBankCardViewModel bankCardViewModel = null;
            
            
            if (transactionViewModel.PaidByType.Name == "Card")
            {
                BankCard bankCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == transactionViewModel.PaidById);
                BankCardStatus cardStatus = await _context.BankCardStatuses.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);
                BankCardType cardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);
                bankCardViewModel = new UserBankCardViewModel()
                {
                    Id = bankCard.Id,
                    CardNumber = bankCard.CardNumber,
                    CVV = bankCard.CVV,
                    ExpiryDate = bankCard.ExpiryDate,
                    Balance = bankCard.Balance,
                    CardStatus = await _context.BankCardStatusModels.FirstOrDefaultAsync(x => x.Id == cardStatus.StatusId),
                    CardType = await _context.BankCardTypeModels.FirstOrDefaultAsync(x => x.Id == cardType.TypeId),

                };
                paidBy.User = await _context.Users.FirstOrDefaultAsync(x => x.Id == bankCard.UserId);
            }
            
            
            UserBankAccountViewModel bankAccountViewModel = null;
            if (transactionViewModel.PaidByType.Name == "BankAccount")
            {            
                BankAccount bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.AccountNumber == transactionViewModel.PaidById);
                BankAccountStatus accountStatus = await _context.BankAccountStatuses.FirstOrDefaultAsync(x => x.AccountId == bankAccount.Id);
                bankAccountViewModel = new()
                {
                    Id = bankAccount.Id,
                    AccountNumber = bankAccount.AccountNumber,
                    IBAN = bankAccount.IBAN,
                    Balance = bankAccount.Balance,
                    AccountStatus = await _context.BankAccountStatusModels.FirstOrDefaultAsync(x => x.Id == accountStatus.StatusId),

                };
                paidBy.User = await _context.Users.FirstOrDefaultAsync(x => x.Id == bankAccount.UserId);
            }


            UserCashBackViewModel wallet = null;
            if (transactionViewModel.PaidByType.Name == "CashBack")
            {
                CashBack cashBack = await _context.CashBacks.FirstOrDefaultAsync(x => x.CashBackNumber == transactionViewModel.PaidById);

                wallet = new()
                {
                    Id = cashBack.Id,
                    Balance = cashBack.Balance,
                    CreatedDate = cashBack.CreatedDate,
                    UpdatedDate = cashBack.UpdatedDate,
                    CashBackNumber = cashBack.CashBackNumber,
                };
                paidBy.User = await _context.Users.FirstOrDefaultAsync(x => x.Id == cashBack.UserId);
            }



            paidBy.Card = bankCardViewModel;
            paidBy.Account = bankAccountViewModel;
            paidBy.CashBack = wallet;

            //destination


            PaymentDetailsViewModel destination = new();



            UserBankCardViewModel destinationBankCardViewModel = null;


            if (transactionViewModel.DestinationType.Name == "Card")
            {
                BankCard destinationBankCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == transactionViewModel.DestinationId);
                BankCardStatus destinationCardStatus = await _context.BankCardStatuses.FirstOrDefaultAsync(x => x.CardId == destinationBankCard.Id);
                BankCardType destinationCardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == destinationBankCard.Id);
                destinationBankCardViewModel = new UserBankCardViewModel()
                {
                    Id = destinationBankCard.Id,
                    CardNumber = destinationBankCard.CardNumber,
                    CVV = destinationBankCard.CVV,
                    ExpiryDate = destinationBankCard.ExpiryDate,
                    Balance = destinationBankCard.Balance,
                    CardStatus = await _context.BankCardStatusModels.FirstOrDefaultAsync(x => x.Id == destinationCardStatus.StatusId),
                    CardType = await _context.BankCardTypeModels.FirstOrDefaultAsync(x => x.Id == destinationCardType.TypeId),

                };
                destination.User = await _context.Users.FirstOrDefaultAsync(x => x.Id == destinationBankCard.UserId);
            }


            UserBankAccountViewModel destinationBankAccountViewModel = null;
            if (transactionViewModel.DestinationType.Name == "BankAccount")
            {
                BankAccount destinationBankAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.AccountNumber == transactionViewModel.DestinationId);
                BankAccountStatus destinationAccountStatus = await _context.BankAccountStatuses.FirstOrDefaultAsync(x => x.AccountId == destinationBankAccount.Id);
                destinationBankAccountViewModel = new()
                {
                    Id = destinationBankAccount.Id,
                    AccountNumber = destinationBankAccount.AccountNumber,
                    IBAN = destinationBankAccount.IBAN,
                    Balance = destinationBankAccount.Balance,
                    AccountStatus = await _context.BankAccountStatusModels.FirstOrDefaultAsync(x => x.Id == destinationAccountStatus.StatusId),

                };
                destination.User = await _context.Users.FirstOrDefaultAsync(x => x.Id == destinationBankAccount.UserId);

            }







            destination.Card = destinationBankCardViewModel;
            destination.Account = destinationBankAccountViewModel;
            

            //end

            ViewData["Destination"] = destination;
            ViewData["PaidBy"] = paidBy;
            ViewData["TransactionViewModel"] = transactionViewModel;
            TempData["Tab"] = "Payments";


            return View();
        }
    }
}
