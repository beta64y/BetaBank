using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetaBank.Services.Implementations;
using BetaBank.Services.Validators;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Win32.SafeHandles;
namespace BetaBank.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly BetaBankDbContext _context;
        private readonly ExternalDbContext _externalContext;

        public PaymentController(BetaBankDbContext context, UserManager<AppUser> userManager, ExternalDbContext externalContext)
        {

            _context = context;
            _userManager = userManager;
            _externalContext = externalContext;
        }


        //public async Task<IActionResult> TransferToOwnCard()
        //public async Task<IActionResult> TransferToSubscription()
        //public async Task<IActionResult> TransferToOwnAccount()
        //public async Task<IActionResult> TransferToPhoneNumber()
        //public async Task<IActionResult> TransferToBakuCard()
        public async Task<IActionResult> TransferToCard()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            List<BankCard> bankCards = await _context.BankCards.Where(x => x.UserId == user.Id).ToListAsync();
            List<BankCardViewModel> bankCardsViewModel = new List<BankCardViewModel>();
            foreach (var bankCard in bankCards)
            {
                BankCardType cardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);
                
                if (await bankCard.IsDisabled(_context) || await bankCard.IsBlocked(_context))
                {
                    continue;
                }
                bankCardsViewModel.Add(new BankCardViewModel
                {
                    Id = bankCard.Id,
                    Balance = bankCard.Balance,
                    CardNumber = bankCard.CardNumber,
                    CardType = (await _context.BankCardTypeModels.FirstOrDefaultAsync(x => x.Id == cardType.TypeId)).Name,

                });
            }

            var cashBack = await _context.CashBacks.FirstOrDefaultAsync(x => x.UserId == user.Id);
            CashBackViewModel cashBackViewModel = new()
            {
                Balance = cashBack.Balance,
                CashBackNumber = cashBack.CashBackNumber
            };

            var bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.UserId == user.Id);


            
                BankAccountViewModel bankAccountViewModel = new()
                {
                    Balance = bankAccount.Balance,
                    AccountNumber = bankAccount.AccountNumber,
                };



            TransferViewModel transferViewModel = new()
            {
                BankCardViewModels = bankCardsViewModel,
                CashBackViewModel = cashBackViewModel.Balance >= 5 ? cashBackViewModel : null,
                BankAccountViewModel = await bankAccount.IsSuspended(_context) ? null : bankAccountViewModel
            };
            if(transferViewModel.BankCardViewModels.Count == 0 && transferViewModel.CashBackViewModel == null && transferViewModel.BankAccountViewModel == null)
            {
                ViewBag.Message = "Ooo, you ain't got no money to spend !";
                return View("Waring");
            }
            ViewData["TransferViewModel"] = transferViewModel;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransferToCard(TransactionViewModel transactionViewModel)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            Models.Transaction transaction;
            if(transactionViewModel.PaidById.Replace(" ", "").Length == 16)
            {
                //card
                var paidByCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == transactionViewModel.PaidById);

                if (paidByCard == null || user.Id != paidByCard.UserId || await paidByCard.IsDisabled(_context) || await paidByCard.IsBlocked(_context))
                {
                    ViewBag.Message = "Card Not Found";
                    return View("Error");
                }
                if(!await paidByCard.CanUseCard(_context))
                {
                    ViewBag.Message = "The card has reached its daily usage limit.";
                    return View("Error");
                }

                var destinationCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == transactionViewModel.DestinationId);

                if (destinationCard == null)
                {
                    var destinationExternalCard = await _externalContext.BankCards.FirstOrDefaultAsync(x => x.CardNumber == transactionViewModel.DestinationId);
                    if (destinationExternalCard == null)
                    {
                        ViewBag.Message = "Destination Card Not Found";
                        return View("Error");
                    }
                    if (paidByCard.Balance < 0.5 + transactionViewModel.Amount)
                    {
                        ViewBag.Message = "Ooo, you ain't got no money to spend !";
                        return View("Waring");
                    }
                    var transactionCardTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Card");
                    transactionViewModel.Commission = 0.5;
                    transaction = new()
                    {
                        Id = $"{Guid.NewGuid()}",
                        ReceiptNumber = ReceiptNumberGenerator.GenerateReceiptNumber(),
                        Amount = transactionViewModel.Amount,
                        Commission = transactionViewModel.Commission,
                        BillingAmount = transactionViewModel.Amount + transactionViewModel.Commission,
                        CashbackAmount = 0,
                        TransactionDate = DateTime.UtcNow,
                        PaidByTypeId = transactionCardTypeModel.Id,
                        PaidById = transactionViewModel.PaidById,
                        DestinationTypeId = transactionCardTypeModel.Id,
                        DestinationId = destinationExternalCard.CardNumber,
                        StatusId = paidByCard.Balance < transactionViewModel.Amount + transactionViewModel.Commission ? (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Completed")).Id : (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Failed")).Id,
                        Icon = destinationExternalCard.Title,
                    };



                    paidByCard.Balance -= transaction.BillingAmount;
                    //destinationExternalCard.Balance += transactionViewModel.Amount; // burani bu formada saxlama sebebi bizim amounttun gedib gedmediyi maraqlandirmamasidir 

                    var cardUsage = new BankCardUsage
                    {
                        Id = Guid.NewGuid().ToString(),
                        CardId = paidByCard.Id,
                        UsageDate = DateTime.UtcNow
                    };
                    await _context.Transactions.AddAsync(transaction);
                    await _context.CardUsages.AddAsync(cardUsage);
                    await _context.SaveChangesAsync();


                    ViewData["Transaction"] = transaction;
                    return View("Transaction");


                }
                else
                {
                    if (await destinationCard.IsDisabled(_context) || await destinationCard.IsBlocked(_context))
                    {
                        ViewBag.Message = "Destination Card Not Found";
                        return View("Error");
                    }
                    if (destinationCard.CardNumber == paidByCard.CardNumber)
                    {
                        ViewBag.Message = "You cannot use the same cards";
                        return View("Error");
                    }
                    if (paidByCard.Balance < 0 + transactionViewModel.Amount)
                    {
                        ViewBag.Message = "Ooo, you ain't got no money to spend !";
                        return View("Waring");
                    }
                    var transactionCardTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Card");
                    transactionViewModel.Commission = 0;
                    transaction = new()
                    {
                        Id = $"{Guid.NewGuid()}",
                        ReceiptNumber = ReceiptNumberGenerator.GenerateReceiptNumber(),
                        Amount = transactionViewModel.Amount,
                        Commission = transactionViewModel.Amount / 100,
                        BillingAmount = transactionViewModel.Amount,
                        CashbackAmount = await paidByCard.IsCardType(_context, "BetaCard") && await destinationCard.IsCardType(_context, "BetaCard") ? transactionViewModel.Amount/2000 : 0,
                        TransactionDate = DateTime.UtcNow,
                        PaidByTypeId = transactionCardTypeModel.Id,
                        PaidById = transactionViewModel.PaidById,
                        DestinationTypeId = transactionCardTypeModel.Id,
                        DestinationId = destinationCard.CardNumber,
                        StatusId = paidByCard.Balance >= transactionViewModel.Amount + transactionViewModel.Commission ? (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Completed")).Id : (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Failed")).Id,
                        Icon = "BetaBank",
                    };
                    paidByCard.Balance -= transaction.BillingAmount;
                    destinationCard.Balance += transactionViewModel.Amount;
                    var paidByCashBack = await _context.CashBacks.FirstOrDefaultAsync(x => x.UserId == user.Id);
                    paidByCashBack.Balance += transaction.CashbackAmount;


                    var cardUsage = new BankCardUsage
                    {
                        Id = Guid.NewGuid().ToString(),
                        CardId = paidByCard.Id,
                        UsageDate = DateTime.UtcNow
                    };
                    await _context.Transactions.AddAsync(transaction);
                    await _context.CardUsages.AddAsync(cardUsage);
                    await _context.SaveChangesAsync();


                    ViewData["Transaction"] = transaction;
                    return View("Transaction");
                }
            }
            else if (transactionViewModel.PaidById.Replace(" ", "").Length == 10)
            {
                //account
                var paidByBankAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.AccountNumber == transactionViewModel.PaidById);
                if (paidByBankAccount == null || user.Id != paidByBankAccount.UserId || await paidByBankAccount.IsSuspended(_context))
                {
                    ViewBag.Message = "Bank Account Not Found";
                    return View("Error");
                }

                var destinationCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == transactionViewModel.DestinationId);

                if (destinationCard == null)
                {
                    var destinationExternalCard = await _externalContext.BankCards.FirstOrDefaultAsync(x => x.CardNumber == transactionViewModel.DestinationId);
                    if (destinationExternalCard == null)
                    {
                        ViewBag.Message = "Destination Card Not Found";
                        return View("Error");
                    }
                    if (paidByBankAccount.Balance < 0.5 + transactionViewModel.Amount)
                    {
                        ViewBag.Message = "Ooo, you ain't got no money to spend !";
                        return View("Waring");
                    }
                    var transactionCardTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Card");
                    var transactionBankAccountTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "BankAccount");
                    transactionViewModel.Commission = 0.5;
                    transaction = new()
                    {
                        Id = $"{Guid.NewGuid()}",
                        ReceiptNumber = ReceiptNumberGenerator.GenerateReceiptNumber(),
                        Amount = transactionViewModel.Amount,
                        Commission = transactionViewModel.Commission,
                        BillingAmount = transactionViewModel.Amount + transactionViewModel.Commission,
                        CashbackAmount = 0,
                        TransactionDate = DateTime.UtcNow,
                        PaidByTypeId = transactionBankAccountTypeModel.Id,
                        PaidById = transactionViewModel.PaidById,
                        DestinationTypeId = transactionCardTypeModel.Id,
                        DestinationId = destinationExternalCard.CardNumber,
                        StatusId = paidByBankAccount.Balance >= transactionViewModel.Amount + transactionViewModel.Commission ? (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Completed")).Id : (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Failed")).Id,
                        Icon = destinationExternalCard.Title,
                    };



                    paidByBankAccount.Balance -= transaction.BillingAmount;
                    //destinationExternalCard.Balance += transactionViewModel.Amount; // burani bu formada saxlama sebebi bizim amounttun gedib gedmediyi maraqlandirmamasidir 

                    
                    await _context.Transactions.AddAsync(transaction);
                    await _context.SaveChangesAsync();


                    ViewData["Transaction"] = transaction;
                    return View("Transaction");


                }
                else
                {
                    if (await destinationCard.IsDisabled(_context) || await destinationCard.IsBlocked(_context))
                    {
                        ViewBag.Message = "Destination Card Not Found";
                        return View("Error");
                    }
                    if (paidByBankAccount.Balance < 0 + transactionViewModel.Amount)
                    {
                        ViewBag.Message = "Ooo, you ain't got no money to spend !";
                        return View("Waring");
                    }
                    var transactionCardTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Card");
                    var transactionBankAccountTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "BankAccount");
                    transactionViewModel.Commission = 0;
                    transaction = new()
                    {
                        Id = $"{Guid.NewGuid()}",
                        ReceiptNumber = ReceiptNumberGenerator.GenerateReceiptNumber(),
                        Amount = transactionViewModel.Amount,
                        Commission = transactionViewModel.Commission,
                        BillingAmount = transactionViewModel.Amount + transactionViewModel.Commission,
                        CashbackAmount = 0,
                        TransactionDate = DateTime.UtcNow,
                        PaidByTypeId = transactionBankAccountTypeModel.Id,
                        PaidById = transactionViewModel.PaidById,
                        DestinationTypeId = transactionCardTypeModel.Id,
                        DestinationId = destinationCard.CardNumber,
                        StatusId = paidByBankAccount.Balance >= transactionViewModel.Amount + transactionViewModel.Commission ? (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Completed")).Id : (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Failed")).Id,
                        Icon = "BetaBank",
                    };

                    paidByBankAccount.Balance -= transaction.BillingAmount;
                    destinationCard.Balance += transactionViewModel.Amount;
                   
                    await _context.Transactions.AddAsync(transaction);
                    await _context.SaveChangesAsync();


                    ViewData["Transaction"] = transaction;
                    return View("Transaction");
                }
            }
            else if (transactionViewModel.PaidById.Replace(" ", "").Length == 15)
            {
                //cashback
                var paidByCashBack = await _context.CashBacks.FirstOrDefaultAsync(x => x.CashBackNumber == transactionViewModel.PaidById);
                if (paidByCashBack == null || user.Id != paidByCashBack.UserId)
                {
                    ViewBag.Message = "CashBack Wallet Not Found";
                    return View("Error");
                }
                if(paidByCashBack.Balance < 5)
                {
                    ViewBag.Message = "You need to have at least 5 dollars to use CashBack Wallet";
                    return View("Waring");
                }

                var destinationCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == transactionViewModel.DestinationId);

                if (destinationCard == null)
                {
                    var destinationExternalCard = await _externalContext.BankCards.FirstOrDefaultAsync(x => x.CardNumber == transactionViewModel.DestinationId);
                    if (destinationExternalCard == null)
                    {
                        ViewBag.Message = "Destination Card Not Found";
                        return View("Error");
                    }
                    if (paidByCashBack.Balance < transactionViewModel.Commission + transactionViewModel.Amount)
                    {
                        ViewBag.Message = "Ooo, you ain't got no money to spend !";
                        return View("Waring");
                    }
                    var transactionCardTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Card");
                    var transactionCashBackTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "CashBack");
                    transactionViewModel.Commission = 0.5;
                    transaction = new()
                    {
                        Id = $"{Guid.NewGuid()}",
                        ReceiptNumber = ReceiptNumberGenerator.GenerateReceiptNumber(),
                        Amount = transactionViewModel.Amount,
                        Commission = transactionViewModel.Commission,
                        BillingAmount = transactionViewModel.Amount + transactionViewModel.Commission,
                        CashbackAmount = 0,
                        TransactionDate = DateTime.UtcNow,
                        PaidByTypeId = transactionCashBackTypeModel.Id,
                        PaidById = transactionViewModel.PaidById,
                        DestinationTypeId = transactionCardTypeModel.Id,
                        DestinationId = destinationExternalCard.CardNumber,
                        StatusId = paidByCashBack.Balance >= transactionViewModel.Amount + transactionViewModel.Commission ? (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Completed")).Id : (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Failed")).Id,
                        Icon = destinationExternalCard.Title,
                    };



                    paidByCashBack.Balance -= transaction.BillingAmount;
                    //destinationExternalCard.Balance += transactionViewModel.Amount; // burani bu formada saxlama sebebi bizim amounttun gedib gedmediyi maraqlandirmamasidir 

                    
                    await _context.Transactions.AddAsync(transaction);
                    await _context.SaveChangesAsync();

                    ViewData["Transaction"] = transaction;
                    return View("Transaction");


                }
                else
                {
                    if (await destinationCard.IsDisabled(_context) || await destinationCard.IsBlocked(_context))
                    {
                        ViewBag.Message = "Destination Card Not Found";
                        return View("Error");
                    }
                    if (paidByCashBack.Balance < 0 + transactionViewModel.Amount)
                    {
                        ViewBag.Message = "Ooo, you ain't got no money to spend !";
                        return View("Waring");
                    }
                    var transactionCardTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Card");
                    var transactionCashBackTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "CashBack");
                    transactionViewModel.Commission = 0;
                    transaction = new()
                    {
                        Id = $"{Guid.NewGuid()}",
                        ReceiptNumber = ReceiptNumberGenerator.GenerateReceiptNumber(),
                        Amount = transactionViewModel.Amount,
                        Commission = transactionViewModel.Amount / 100,
                        BillingAmount = transactionViewModel.Amount,
                        CashbackAmount = 0,
                        TransactionDate = DateTime.UtcNow,
                        PaidByTypeId = transactionCashBackTypeModel.Id,
                        PaidById = transactionViewModel.PaidById,
                        DestinationTypeId = transactionCardTypeModel.Id,
                        DestinationId = destinationCard.CardNumber,
                        StatusId = paidByCashBack.Balance >= transactionViewModel.Amount + transactionViewModel.Commission ? (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Completed")).Id : (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Failed")).Id,
                        Icon = "BetaBank",
                    };
                    paidByCashBack.Balance -= transaction.BillingAmount;
                    destinationCard.Balance += transactionViewModel.Amount;


                    
                    await _context.Transactions.AddAsync(transaction);
                    await _context.SaveChangesAsync();

                    ViewData["Transaction"] = transaction;
                    return View("Transaction");
                }
            }
            else
            {
                ViewBag.Message = "Something went wrong!";
                return View("Error");
            }

        }




        public async Task<IActionResult> TransferToSubscription()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            List<BankCard> bankCards = await _context.BankCards.Where(x => x.UserId == user.Id).ToListAsync();
            List<BankCardViewModel> bankCardsViewModel = new List<BankCardViewModel>();
            foreach (var bankCard in bankCards)
            {
                BankCardType cardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);

                if (await bankCard.IsDisabled(_context) || await bankCard.IsBlocked(_context))
                {
                    continue;
                }
                bankCardsViewModel.Add(new BankCardViewModel
                {
                    Id = bankCard.Id,
                    Balance = bankCard.Balance,
                    CardNumber = bankCard.CardNumber,
                    CardType = (await _context.BankCardTypeModels.FirstOrDefaultAsync(x => x.Id == cardType.TypeId)).Name,

                });
            }

            var cashBack = await _context.CashBacks.FirstOrDefaultAsync(x => x.UserId == user.Id);
            CashBackViewModel cashBackViewModel = new()
            {
                Balance = cashBack.Balance,
                CashBackNumber = cashBack.CashBackNumber
            };

            var bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.UserId == user.Id);



            BankAccountViewModel bankAccountViewModel = new()
            {
                Balance = bankAccount.Balance,
                AccountNumber = bankAccount.AccountNumber,
            };



            TransferViewModel transferViewModel = new()
            {
                BankCardViewModels = bankCardsViewModel,
                CashBackViewModel = cashBackViewModel.Balance >= 5 ? cashBackViewModel : null,
                BankAccountViewModel = await bankAccount.IsSuspended(_context) ? null : bankAccountViewModel
            };
            if (transferViewModel.BankCardViewModels.Count == 0 && transferViewModel.CashBackViewModel == null && transferViewModel.BankAccountViewModel == null)
            {
                ViewBag.Message = "Ooo, you ain't got no money to spend !";
                return View("Waring");
            }
            ViewData["TransferViewModel"] = transferViewModel;

            return View();
        }

         Subscriptions









        //hamisi ucun
        public async Task<IActionResult> Transaction()
        {
            return View();
        }








    }
}
