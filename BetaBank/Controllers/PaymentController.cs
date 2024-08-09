using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetaBank.Services.Implementations;
using BetaBank.Services.Validators;
using Microsoft.IdentityModel.Tokens;
using BetaBank.Utils.Enums;

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


        //public async Task<IActionResult> TransferToOwnCard()++++++++++++++++++++
        //public async Task<IActionResult> TransferToSubscription()+++++++++++++++++++
        //public async Task<IActionResult> TransferToOwnAccount()++++++++++++++++++++
        //public async Task<IActionResult> TransferToPhoneNumber()
        //public async Task<IActionResult> TransferToBakuCard()


        /* Start Tranfer Another Card */
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
                Models.BankCardType cardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);

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
            if (/*transferViewModel.BankCardViewModels.Count == 0 &&*/ transferViewModel.CashBackViewModel == null && transferViewModel.BankAccountViewModel == null)
            {
                ViewBag.Message = "Ooo, you ain't got no money to spend !";
                return View("Warning");
            }
            ViewData["TransferViewModel"] = transferViewModel;

            return View();
        }

        

        /* End Tranfer Another Card */

        /* Start Tranfer Own Card */
        public async Task<IActionResult> TransferToOwnCard()
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
                Models.BankCardType cardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);

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
            if (bankCardsViewModel.Count == 0)
            {
                ViewBag.Message = "You have no available cards.";
                return View("Warning");
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
                return View("Warning");
            }
            ViewData["TransferViewModel"] = transferViewModel;

            return View();
        }

        //TransferToCard [Post]
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
            
            if(transactionViewModel.DestinationId==null || transactionViewModel.PaidById == null || transactionViewModel.Amount == null)
            {
                ViewBag.Message = "Something went wrong!";
                return View("Error");
            }if(transactionViewModel.Amount < 2)
            {
                ViewBag.Message = "Amount cannot be less than $2!";
                return View("Error");
            }
            Models.Transaction transaction;
            transactionViewModel.DestinationId = transactionViewModel.DestinationId.Replace(" ", "");
            transactionViewModel.PaidById = transactionViewModel.PaidById.Replace(" ", "");
            if (transactionViewModel.PaidById.Length == 16)
            {
                //card
                var paidByCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == transactionViewModel.PaidById);

                if (paidByCard == null || user.Id != paidByCard.UserId || await paidByCard.IsDisabled(_context) || await paidByCard.IsBlocked(_context))
                {
                    ViewBag.Message = "Card Not Found";
                    return View("Error");
                }
                if (!await paidByCard.CanUseCard(_context))
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
                        return View("Warning");
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
                        StatusId = paidByCard.Balance > transactionViewModel.Amount + transactionViewModel.Commission ? (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Completed")).Id : (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Failed")).Id,
                        Title = "Card",
                        Description = destinationExternalCard.Title,
                    };
                    if (transaction.StatusId == "Completed")
                    {
                        paidByCard.Balance -= transaction.BillingAmount;
                        //destinationExternalCard.Balance += transactionViewModel.Amount; // burani bu formada saxlama sebebi bizim amounttun gedib gedmediyi maraqlandirmamasidir 

                        var cardUsage = new BankCardUsage
                        {
                            Id = Guid.NewGuid().ToString(),
                            CardId = paidByCard.Id,
                            UsageDate = DateTime.UtcNow
                        }; await _context.CardUsages.AddAsync(cardUsage);

                    }
                    else if (transaction.StatusId == "Failed")
                    {

                    }
                    await _context.Transactions.AddAsync(transaction);
                    await _context.SaveChangesAsync();


                    
                    return RedirectToAction("Transaction" , new { id = transaction.Id });


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
                        return View("Warning");
                    }
                    var transactionCardTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Card");
                    transactionViewModel.Commission = 0;
                    transaction = new()
                    {
                        Id = $"{Guid.NewGuid()}",
                        ReceiptNumber = ReceiptNumberGenerator.GenerateReceiptNumber(),
                        Amount = transactionViewModel.Amount,
                        Commission = transactionViewModel.Commission,
                        BillingAmount = transactionViewModel.Amount + transactionViewModel.Commission,
                        CashbackAmount = await paidByCard.IsCardType(_context, "BetaCard") && await destinationCard.IsCardType(_context, "BetaCard") ? transactionViewModel.Amount / 2000 : 0,
                        TransactionDate = DateTime.UtcNow,
                        PaidByTypeId = transactionCardTypeModel.Id,
                        PaidById = transactionViewModel.PaidById,
                        DestinationTypeId = transactionCardTypeModel.Id,
                        DestinationId = destinationCard.CardNumber,
                        StatusId = paidByCard.Balance >= transactionViewModel.Amount + transactionViewModel.Commission ? (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Completed")).Id : (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Failed")).Id,
                        Title = "Card",
                        Description = "BetaBank",
                    };
                    if (transaction.StatusId == "Completed")
                    {
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
                        await _context.CardUsages.AddAsync(cardUsage);

                    }
                    else if (transaction.StatusId == "Failed")
                    {

                    }
                   
                    await _context.Transactions.AddAsync(transaction);
                    await _context.SaveChangesAsync();


                    return RedirectToAction("Transaction" , new { id = transaction.Id });

                }
            }
            else if (transactionViewModel.PaidById.Length == 10)
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
                        return View("Warning");
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
                        Title = "Card",
                        Description = destinationExternalCard.Title,
                    };
                    if (transaction.StatusId == "Completed")
                    {
                      paidByBankAccount.Balance -= transaction.BillingAmount;
                    //destinationExternalCard.Balance += transactionViewModel.Amount; // burani bu formada saxlama sebebi bizim amounttun gedib gedmediyi maraqlandirmamasidir 

                    }
                    else if (transaction.StatusId == "Failed")
                    {

                    }


                   


                    await _context.Transactions.AddAsync(transaction);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Transaction" , new { id = transaction.Id });



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
                        return View("Warning");
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
                        Title = "Card",
                        Description = "BetaBank",
                    };
                    if (transaction.StatusId == "Completed")
                    {
paidByBankAccount.Balance -= transaction.BillingAmount;
                    destinationCard.Balance += transactionViewModel.Amount;

                    }
                    else if (transaction.StatusId == "Failed")
                    {

                    }
                    

                    await _context.Transactions.AddAsync(transaction);
                    await _context.SaveChangesAsync();


                    return RedirectToAction("Transaction" , new { id = transaction.Id });

                }
            }
            else if (transactionViewModel.PaidById.Length == 15)
            {
                //cashback
                var paidByCashBack = await _context.CashBacks.FirstOrDefaultAsync(x => x.CashBackNumber == transactionViewModel.PaidById);
                if (paidByCashBack == null || user.Id != paidByCashBack.UserId)
                {
                    ViewBag.Message = "CashBack Wallet Not Found";
                    return View("Error");
                }
                if (paidByCashBack.Balance < 5)
                {
                    ViewBag.Message = "You need to have at least 5 dollars to use CashBack Wallet";
                    return View("Warning");
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
                    if (paidByCashBack.Balance < 0.5 + transactionViewModel.Amount)
                    {
                        ViewBag.Message = "Ooo, you ain't got no money to spend !";
                        return View("Warning");
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
                        Title = "Card",
                        Description = destinationExternalCard.Title,
                    };
                    if (transaction.StatusId == "Completed")
                    {
                     paidByCashBack.Balance -= transaction.BillingAmount;
                    //destinationExternalCard.Balance += transactionViewModel.Amount; // burani bu formada saxlama sebebi bizim amounttun gedib gedmediyi maraqlandirmamasidir 


                    }
                    else if (transaction.StatusId == "Failed")
                    {

                    }


                   

                    await _context.Transactions.AddAsync(transaction);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Transaction" , new { id = transaction.Id });



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
                        return View("Warning");
                    }
                    var transactionCardTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Card");
                    var transactionCashBackTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "CashBack");
                    transactionViewModel.Commission = 0;
                    transaction = new()
                    {
                        Id = $"{Guid.NewGuid()}",
                        ReceiptNumber = ReceiptNumberGenerator.GenerateReceiptNumber(),
                        Amount = transactionViewModel.Amount,
                        Commission = transactionViewModel.Commission,
                        BillingAmount = transactionViewModel.Amount,
                        CashbackAmount = 0,
                        TransactionDate = DateTime.UtcNow,
                        PaidByTypeId = transactionCashBackTypeModel.Id,
                        PaidById = transactionViewModel.PaidById,
                        DestinationTypeId = transactionCardTypeModel.Id,
                        DestinationId = destinationCard.CardNumber,
                        StatusId = paidByCashBack.Balance >= transactionViewModel.Amount + transactionViewModel.Commission ? (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Completed")).Id : (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Failed")).Id,
                        Title = "Card",
                        Description = "BetaBank",
                    };
                    if (transaction.StatusId == "Completed")
                    {
paidByCashBack.Balance -= transaction.BillingAmount;
                    destinationCard.Balance += transactionViewModel.Amount;


                    }
                    else if (transaction.StatusId == "Failed")
                    {

                    }
                    


                    await _context.Transactions.AddAsync(transaction);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Transaction" , new { id = transaction.Id });

                }
            }
            else
            {
                ViewBag.Message = "Something went wrong!";
                return View("Error");
            }

        }
        /* End Tranfer Own Card */





        /* Start Tranfer Own Account */
        public async Task<IActionResult> TransferToOwnAccount()
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
                Models.BankCardType cardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);

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
            if (/*transferViewModel.BankCardViewModels.Count == 0 &&*/ transferViewModel.CashBackViewModel == null && transferViewModel.BankAccountViewModel == null)
            {
                ViewBag.Message = "Ooo, you ain't got no money to spend !";
                return View("Warning");
            }
            ViewData["TransferViewModel"] = transferViewModel;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransferToOwnAccount(TransactionViewModel transactionViewModel)
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
            
            if (transactionViewModel.PaidById == null || transactionViewModel.Amount == null)
            {
                ViewBag.Message = "Something went wrong!";
                return View("Error");
            }
if (transactionViewModel.Amount < 2)
            {
                ViewBag.Message = "Amount cannot be less than $2!";
                return View("Error");
            }

            Models.Transaction transaction;
            if (transactionViewModel.PaidById.Replace(" ", "").Length == 16)
            {
                //card
                var paidByCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == transactionViewModel.PaidById);

                if (paidByCard == null || user.Id != paidByCard.UserId || await paidByCard.IsDisabled(_context) || await paidByCard.IsBlocked(_context))
                {
                    ViewBag.Message = "Card Not Found";
                    return View("Error");
                }
                if (!await paidByCard.CanUseCard(_context))
                {
                    ViewBag.Message = "The card has reached its daily usage limit.";
                    return View("Error");
                }

                var destinationAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (destinationAccount == null || await destinationAccount.IsSuspended(_context)  )
                {
                    ViewBag.Message = "Destination Account Not Found";
                    return View("Error");
                }
                if (paidByCard.Balance < 0 + transactionViewModel.Amount)
                {
                    ViewBag.Message = "Ooo, you ain't got no money to spend !";
                    return View("Warning");
                }
                var transactionCardTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Card");
                var transactionAccountTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "BankAccount");
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
                    PaidByTypeId = transactionCardTypeModel.Id,
                    PaidById = transactionViewModel.PaidById,
                    DestinationTypeId = transactionAccountTypeModel.Id,
                    DestinationId = destinationAccount.AccountNumber,
                    StatusId = paidByCard.Balance >= transactionViewModel.Amount + transactionViewModel.Commission ? (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Completed")).Id : (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Failed")).Id,
                    Title = "BankAccount",
                    Description = "BetaBank",
                };
                if (transaction.StatusId == "Completed")
                {
paidByCard.Balance -= transaction.BillingAmount;
                destinationAccount.Balance += transactionViewModel.Amount;

                var cardUsage = new BankCardUsage
                {
                    Id = Guid.NewGuid().ToString(),
                    CardId = paidByCard.Id,
                    UsageDate = DateTime.UtcNow
                };                await _context.CardUsages.AddAsync(cardUsage);


                }
                else if (transaction.StatusId == "Failed")
                {

                }
                
                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();


                return RedirectToAction("Transaction" , new { id = transaction.Id });


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
                if (paidByCashBack.Balance < 5)
                {
                    ViewBag.Message = "You need to have at least 5 dollars to use CashBack Wallet";
                    return View("Warning");
                }

                var destinationAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (destinationAccount == null || await destinationAccount.IsSuspended(_context) || destinationAccount.UserId != user.Id)
                {
                    ViewBag.Message = "Destination Account Not Found";
                    return View("Error");
                }


                if (paidByCashBack.Balance < 0 + transactionViewModel.Amount)
                {
                    ViewBag.Message = "Ooo, you ain't got no money to spend !";
                    return View("Warning");
                }

                var transactionAccountTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "BankAccount");
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
                    DestinationTypeId = transactionAccountTypeModel.Id,
                    DestinationId = destinationAccount.AccountNumber,
                    StatusId = paidByCashBack.Balance >= transactionViewModel.Amount + transactionViewModel.Commission ? (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Completed")).Id : (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Failed")).Id,
                    Title = "BankAccount",
                    Description = "BetaBank",
                }; if (transaction.StatusId == "Completed")
                {
 paidByCashBack.Balance -= transaction.BillingAmount;
                destinationAccount.Balance += transactionViewModel.Amount;


                }
                else if (transaction.StatusId == "Failed")
                {

                }
               


                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();

                return RedirectToAction("Transaction" , new { id = transaction.Id });


            }
            else
            {
                ViewBag.Message = "Something went wrong!";
                return View("Error");
            }

        }

        /* End Tranfer Own Account */






        /* Start Tranfer Own Subscription */
        public async Task<IActionResult> Subscriptions(string id)
        {
            TransactionTypeModel transactionTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(t => t.Id == id);
            if(transactionTypeModel.Name == "Utility")
            {
                List<UtilityModel> utilities = await _context.UtilityModels.OrderBy(x => x.Name).ToListAsync();
                List<SubscriptionsPaymentViewModel> subscriptionsPaymentViewModels = new();
                foreach (UtilityModel utility in utilities)
                {
                    subscriptionsPaymentViewModels.Add(new SubscriptionsPaymentViewModel { Id = utility.Id, Name = utility.Name });
                }

                ViewData["SubscriptionsPaymentViewModel"] = subscriptionsPaymentViewModels;
                ViewData["Subscription"] = "Utility";
                return View();

            }
            else if (transactionTypeModel.Name == "Internet")
            {
                List<InternetModel> internetProviders = await _context.InternetModels.OrderBy(x => x.Name).ToListAsync();
                List<SubscriptionsPaymentViewModel> subscriptionsPaymentViewModels = new();
                foreach(InternetModel internet in internetProviders)
                {
                    subscriptionsPaymentViewModels.Add(new SubscriptionsPaymentViewModel { Id = internet.Id, Name = internet.Name });
                }    
                ViewData["SubscriptionsPaymentViewModel"] = subscriptionsPaymentViewModels;
                ViewData["Subscription"] = "Internet";
                return View();
            }
            else
            {
                ViewBag.Message = "Something went wrong!";
                return View("Error");
            }
        }

        /*  ----------------------------------------- */



        /* Start Tranfer Own UtilitySubscription */
        public async Task<IActionResult> UtilitySubscriptionTransaction(string id)
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
                Models.BankCardType cardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);

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
            if (/*transferViewModel.BankCardViewModels.Count == 0 &&*/ transferViewModel.CashBackViewModel == null && transferViewModel.BankAccountViewModel == null)
            {
                ViewBag.Message = "Ooo, you ain't got no money to spend !";
                return View("Warning");
            }
            


            /*tek elave*/
            UtilityModel utilityModel = await _context.UtilityModels.FirstOrDefaultAsync(x => x.Id == id);
            if(utilityModel == null)
            {
                ViewBag.Message = "Something went wrong!";
                return View("Error");
            }



            ViewData["TransferViewModel"] = transferViewModel;
            ViewData["UtilityModel"] = utilityModel;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UtilitySubscriptionTransaction(SubscriptionPaymentViewModel subscriptionPaymentViewModel)
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

            if (subscriptionPaymentViewModel.DestinationId == null || subscriptionPaymentViewModel.PaidById == null || subscriptionPaymentViewModel.Amount == null && (subscriptionPaymentViewModel.AppointmentType == "Individuals" || subscriptionPaymentViewModel.AppointmentType == "Commercial consumers" || subscriptionPaymentViewModel.AppointmentType == null))
            {
                ViewBag.Message = "Something went wrong!";
                return View("Error");
            }
            if (subscriptionPaymentViewModel.Amount < 1)
            {
                ViewBag.Message = "Amount cannot be less than $1!";
                return View("Error");
            }
            Models.Transaction transaction;
            subscriptionPaymentViewModel.DestinationId = subscriptionPaymentViewModel.DestinationId.Replace(" ", "");
            subscriptionPaymentViewModel.PaidById = subscriptionPaymentViewModel.PaidById.Replace(" ", "");
            if (subscriptionPaymentViewModel.PaidById.Length == 16)
            {
                //card
                var paidByCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == subscriptionPaymentViewModel.PaidById);

                if (paidByCard == null || user.Id != paidByCard.UserId || await paidByCard.IsDisabled(_context) || await paidByCard.IsBlocked(_context))
                {
                    ViewBag.Message = "Card Not Found";
                    return View("Error");
                }
                if (!await paidByCard.CanUseCard(_context))
                {
                    ViewBag.Message = "The card has reached its daily usage limit.";
                    return View("Error");
                }

                var subscription = await _externalContext.Utilities.FirstOrDefaultAsync(x => x.SubscriberCode == subscriptionPaymentViewModel.DestinationId && x.AppointmentType == subscriptionPaymentViewModel.AppointmentType);

                if (subscription != null)
                {

                    if (paidByCard.Balance < subscriptionPaymentViewModel.Amount)
                    {
                        ViewBag.Message = "Ooo, you ain't got no money to spend !";
                        return View("Warning");
                    }

                    var transactionCardTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Card");
                    var transactionUtilityTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Utility");
                    subscriptionPaymentViewModel.Commission = 0;
                    transaction = new()
                    {
                        Id = $"{Guid.NewGuid()}",
                        ReceiptNumber = ReceiptNumberGenerator.GenerateReceiptNumber(),
                        Amount = subscriptionPaymentViewModel.Amount,
                        Commission = subscriptionPaymentViewModel.Commission,
                        BillingAmount = subscriptionPaymentViewModel.Amount + subscriptionPaymentViewModel.Commission,
                        CashbackAmount = 0,
                        TransactionDate = DateTime.UtcNow,
                        PaidByTypeId = transactionCardTypeModel.Id,
                        PaidById = subscriptionPaymentViewModel.PaidById,
                        DestinationTypeId = transactionUtilityTypeModel.Id,
                        DestinationId = subscription.SubscriberCode,
                        StatusId = paidByCard.Balance > subscriptionPaymentViewModel.Amount + subscriptionPaymentViewModel.Commission ? (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Completed")).Id : (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Failed")).Id,
                        Title = "Utility",
                        Description = subscription.Title,
                    };
                    if (transaction.StatusId == "Completed")
                    {
                        paidByCard.Balance -= transaction.BillingAmount;
                        //destinationExternalCard.Balance += transactionViewModel.Amount; // burani bu formada saxlama sebebi bizim amounttun gedib gedmediyi maraqlandirmamasidir 

                        var cardUsage = new BankCardUsage
                        {
                            Id = Guid.NewGuid().ToString(),
                            CardId = paidByCard.Id,
                            UsageDate = DateTime.UtcNow
                        }; await _context.CardUsages.AddAsync(cardUsage);

                    }
                    else if (transaction.StatusId == "Failed")
                    {

                    }
                    await _context.Transactions.AddAsync(transaction);
                    await _context.SaveChangesAsync();



                    return RedirectToAction("Transaction", new { id = transaction.Id });


                }
                else
                {
                    ViewBag.Message = "Subscription Not Found";
                    return View("Error");

                }
            }
            else if (subscriptionPaymentViewModel.PaidById.Length == 10)
            {
                //account
                var paidByBankAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.AccountNumber == subscriptionPaymentViewModel.PaidById);
                if (paidByBankAccount == null || user.Id != paidByBankAccount.UserId || await paidByBankAccount.IsSuspended(_context))
                {
                    ViewBag.Message = "Bank Account Not Found";
                    return View("Error");
                }

                var subscription = await _externalContext.Utilities.FirstOrDefaultAsync(x => x.SubscriberCode == subscriptionPaymentViewModel.DestinationId && x.AppointmentType == subscriptionPaymentViewModel.AppointmentType);

                if (subscription != null)
                {

                    if (paidByBankAccount.Balance < subscriptionPaymentViewModel.Amount)
                    {
                        ViewBag.Message = "Ooo, you ain't got no money to spend !";
                        return View("Warning");
                    }

                    var transactionUtilityTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Utility");
                    var transactionBankAccountTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "BankAccount");
                    subscriptionPaymentViewModel.Commission = 0;
                    transaction = new()
                    {
                        Id = $"{Guid.NewGuid()}",
                        ReceiptNumber = ReceiptNumberGenerator.GenerateReceiptNumber(),
                        Amount = subscriptionPaymentViewModel.Amount,
                        Commission = subscriptionPaymentViewModel.Commission,
                        BillingAmount = subscriptionPaymentViewModel.Amount + subscriptionPaymentViewModel.Commission,
                        CashbackAmount = 0,
                        TransactionDate = DateTime.UtcNow,
                        PaidByTypeId = transactionBankAccountTypeModel.Id,
                        PaidById = subscriptionPaymentViewModel.PaidById,
                        DestinationTypeId = transactionUtilityTypeModel.Id,
                        DestinationId = subscription.SubscriberCode,
                        StatusId = paidByBankAccount.Balance >= subscriptionPaymentViewModel.Amount + subscriptionPaymentViewModel.Commission ? (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Completed")).Id : (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Failed")).Id,
                        Title = "Utility",
                        Description = subscription.Title,
                    };
                    if (transaction.StatusId == "Completed")
                    {
                        paidByBankAccount.Balance -= transaction.BillingAmount;
                        //destinationExternalCard.Balance += transactionViewModel.Amount; // burani bu formada saxlama sebebi bizim amounttun gedib gedmediyi maraqlandirmamasidir 

                    }
                    else if (transaction.StatusId == "Failed")
                    {

                    }
                    await _context.Transactions.AddAsync(transaction);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Transaction", new { id = transaction.Id });



                }
                else
                {
                    ViewBag.Message = "Subscription Not Found";
                    return View("Error");

                }
            }
            else if (subscriptionPaymentViewModel.PaidById.Length == 15)
            {
                //cashback
                var paidByCashBack = await _context.CashBacks.FirstOrDefaultAsync(x => x.CashBackNumber == subscriptionPaymentViewModel.PaidById);
                if (paidByCashBack == null || user.Id != paidByCashBack.UserId)
                {
                    ViewBag.Message = "CashBack Wallet Not Found";
                    return View("Error");
                }
                if (paidByCashBack.Balance < 5)
                {
                    ViewBag.Message = "You need to have at least 5 dollars to use CashBack Wallet";
                    return View("Warning");
                }

                var subscription = await _externalContext.Utilities.FirstOrDefaultAsync(x => x.SubscriberCode == subscriptionPaymentViewModel.DestinationId && x.AppointmentType == subscriptionPaymentViewModel.AppointmentType);

                if (subscription != null)
                {

                    if (paidByCashBack.Balance < subscriptionPaymentViewModel.Amount)
                    {
                        ViewBag.Message = "Ooo, you ain't got no money to spend !";
                        return View("Warning");
                    }
                    var transactionUtilityTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Utility");
                    var transactionCashBackTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "CashBack");
                    subscriptionPaymentViewModel.Commission = 0;
                    transaction = new()
                    {
                        Id = $"{Guid.NewGuid()}",
                        ReceiptNumber = ReceiptNumberGenerator.GenerateReceiptNumber(),
                        Amount = subscriptionPaymentViewModel.Amount,
                        Commission = subscriptionPaymentViewModel.Commission,
                        BillingAmount = subscriptionPaymentViewModel.Amount + subscriptionPaymentViewModel.Commission,
                        CashbackAmount = 0,
                        TransactionDate = DateTime.UtcNow,
                        PaidByTypeId = transactionCashBackTypeModel.Id,
                        PaidById = subscriptionPaymentViewModel.PaidById,
                        DestinationTypeId = transactionUtilityTypeModel.Id,
                        DestinationId = subscriptionPaymentViewModel.PaidById,
                        StatusId = paidByCashBack.Balance >= subscriptionPaymentViewModel.Amount + subscriptionPaymentViewModel.Commission ? (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Completed")).Id : (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Failed")).Id,
                        Title = "Utility",
                        Description = subscription.Title,
                    };
                    if (transaction.StatusId == "Completed")
                    {
                        paidByCashBack.Balance -= transaction.BillingAmount;
                        //destinationExternalCard.Balance += transactionViewModel.Amount; // burani bu formada saxlama sebebi bizim amounttun gedib gedmediyi maraqlandirmamasidir 


                    }
                    else if (transaction.StatusId == "Failed")
                    {

                    }




                    await _context.Transactions.AddAsync(transaction);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Transaction", new { id = transaction.Id });



                }
                else
                {
                    ViewBag.Message = "Subscription Not Found";
                    return View("Error");

                }
            }
            else
            {
                ViewBag.Message = "Something went wrong!";
                return View("Error");
            }
        }

        /* End Tranfer Own UtilitySubscription */



        /* Start Tranfer Own InternetProviderSubscription */
        public async Task<IActionResult> InternetProviderSubscriptionTransaction(string id)
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
                Models.BankCardType cardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);

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
            if (/*transferViewModel.BankCardViewModels.Count == 0 &&*/ transferViewModel.CashBackViewModel == null && transferViewModel.BankAccountViewModel == null)
            {
                ViewBag.Message = "Ooo, you ain't got no money to spend !";
                return View("Warning");
            }



            /*tek elave*/
            InternetModel internetModel = await _context.InternetModels.FirstOrDefaultAsync(x => x.Id == id);
            if (internetModel == null)
            {
                ViewBag.Message = "Something went wrong!";
                return View("Error");
            }



            ViewData["TransferViewModel"] = transferViewModel;
            ViewData["InternetModel"] = internetModel;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InternetProviderSubscriptionTransaction(SubscriptionPaymentViewModel subscriptionPaymentViewModel)
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

            if (subscriptionPaymentViewModel.DestinationId == null || subscriptionPaymentViewModel.PaidById == null || subscriptionPaymentViewModel.Amount == null && (subscriptionPaymentViewModel.AppointmentType == "Individuals" || subscriptionPaymentViewModel.AppointmentType == "Commercial consumers" || subscriptionPaymentViewModel.AppointmentType == null))
            {
                ViewBag.Message = "Something went wrong!";
                return View("Error");
            }
            if (subscriptionPaymentViewModel.Amount < 1)
            {
                ViewBag.Message = "Amount cannot be less than $1!";
                return View("Error");
            }
            Models.Transaction transaction;
            subscriptionPaymentViewModel.DestinationId = subscriptionPaymentViewModel.DestinationId.Replace(" ", "");
            subscriptionPaymentViewModel.PaidById = subscriptionPaymentViewModel.PaidById.Replace(" ", "");
            if (subscriptionPaymentViewModel.PaidById.Length == 16)
            {
                //card
                var paidByCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == subscriptionPaymentViewModel.PaidById);

                if (paidByCard == null || user.Id != paidByCard.UserId || await paidByCard.IsDisabled(_context) || await paidByCard.IsBlocked(_context))
                {
                    ViewBag.Message = "Card Not Found";
                    return View("Error");
                }
                if (!await paidByCard.CanUseCard(_context))
                {
                    ViewBag.Message = "The card has reached its daily usage limit.";
                    return View("Error");
                }

                var subscription = await _externalContext.InternetProviders.FirstOrDefaultAsync(x => x.SubscriberCode == subscriptionPaymentViewModel.DestinationId && x.AppointmentType == subscriptionPaymentViewModel.AppointmentType);

                if (subscription != null)
                {

                    if (paidByCard.Balance < subscriptionPaymentViewModel.Amount)
                    {
                        ViewBag.Message = "Ooo, you ain't got no money to spend !";
                        return View("Warning");
                    }

                    var transactionCardTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Card");
                    var transactionUtilityTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Internet");
                    subscriptionPaymentViewModel.Commission = 0;
                    transaction = new()
                    {
                        Id = $"{Guid.NewGuid()}",
                        ReceiptNumber = ReceiptNumberGenerator.GenerateReceiptNumber(),
                        Amount = subscriptionPaymentViewModel.Amount,
                        Commission = subscriptionPaymentViewModel.Commission,
                        BillingAmount = subscriptionPaymentViewModel.Amount + subscriptionPaymentViewModel.Commission,
                        CashbackAmount = 0,
                        TransactionDate = DateTime.UtcNow,
                        PaidByTypeId = transactionCardTypeModel.Id,
                        PaidById = subscriptionPaymentViewModel.PaidById,
                        DestinationTypeId = transactionUtilityTypeModel.Id,
                        DestinationId = subscription.SubscriberCode,
                        StatusId = paidByCard.Balance > subscriptionPaymentViewModel.Amount + subscriptionPaymentViewModel.Commission ? (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Completed")).Id : (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Failed")).Id,
                        Title = "Internet",
                        Description = subscription.Title,
                    };
                    if (transaction.StatusId == "Completed")
                    {
                        paidByCard.Balance -= transaction.BillingAmount;
                        //destinationExternalCard.Balance += transactionViewModel.Amount; // burani bu formada saxlama sebebi bizim amounttun gedib gedmediyi maraqlandirmamasidir 

                        var cardUsage = new BankCardUsage
                        {
                            Id = Guid.NewGuid().ToString(),
                            CardId = paidByCard.Id,
                            UsageDate = DateTime.UtcNow
                        }; await _context.CardUsages.AddAsync(cardUsage);

                    }
                    else if (transaction.StatusId == "Failed")
                    {

                    }
                    await _context.Transactions.AddAsync(transaction);
                    await _context.SaveChangesAsync();



                    return RedirectToAction("Transaction", new { id = transaction.Id });


                }
                else
                {
                    ViewBag.Message = "Subscription Not Found";
                    return View("Error");

                }
            }
            else if (subscriptionPaymentViewModel.PaidById.Length == 10)
            {
                //account
                var paidByBankAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.AccountNumber == subscriptionPaymentViewModel.PaidById);
                if (paidByBankAccount == null || user.Id != paidByBankAccount.UserId || await paidByBankAccount.IsSuspended(_context))
                {
                    ViewBag.Message = "Bank Account Not Found";
                    return View("Error");
                }

                var subscription = await _externalContext.InternetProviders.FirstOrDefaultAsync(x => x.SubscriberCode == subscriptionPaymentViewModel.DestinationId && x.AppointmentType == subscriptionPaymentViewModel.AppointmentType);

                if (subscription != null)
                {

                    if (paidByBankAccount.Balance < subscriptionPaymentViewModel.Amount)
                    {
                        ViewBag.Message = "Ooo, you ain't got no money to spend !";
                        return View("Warning");
                    }

                    var transactionUtilityTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Internet");
                    var transactionBankAccountTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "BankAccount");
                    subscriptionPaymentViewModel.Commission = 0;
                    transaction = new()
                    {
                        Id = $"{Guid.NewGuid()}",
                        ReceiptNumber = ReceiptNumberGenerator.GenerateReceiptNumber(),
                        Amount = subscriptionPaymentViewModel.Amount,
                        Commission = subscriptionPaymentViewModel.Commission,
                        BillingAmount = subscriptionPaymentViewModel.Amount + subscriptionPaymentViewModel.Commission,
                        CashbackAmount = 0,
                        TransactionDate = DateTime.UtcNow,
                        PaidByTypeId = transactionBankAccountTypeModel.Id,
                        PaidById = subscriptionPaymentViewModel.PaidById,
                        DestinationTypeId = transactionUtilityTypeModel.Id,
                        DestinationId = subscription.SubscriberCode,
                        StatusId = paidByBankAccount.Balance >= subscriptionPaymentViewModel.Amount + subscriptionPaymentViewModel.Commission ? (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Completed")).Id : (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Failed")).Id,
                        Title = "Internet",
                        Description = subscription.Title,
                    };
                    if (transaction.StatusId == "Completed")
                    {
                        paidByBankAccount.Balance -= transaction.BillingAmount;
                        //destinationExternalCard.Balance += transactionViewModel.Amount; // burani bu formada saxlama sebebi bizim amounttun gedib gedmediyi maraqlandirmamasidir 

                    }
                    else if (transaction.StatusId == "Failed")
                    {

                    }
                    await _context.Transactions.AddAsync(transaction);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Transaction", new { id = transaction.Id });



                }
                else
                {
                    ViewBag.Message = "Subscription Not Found";
                    return View("Error");

                }
            }
            else if (subscriptionPaymentViewModel.PaidById.Length == 15)
            {
                //cashback
                var paidByCashBack = await _context.CashBacks.FirstOrDefaultAsync(x => x.CashBackNumber == subscriptionPaymentViewModel.PaidById);
                if (paidByCashBack == null || user.Id != paidByCashBack.UserId)
                {
                    ViewBag.Message = "CashBack Wallet Not Found";
                    return View("Error");
                }
                if (paidByCashBack.Balance < 5)
                {
                    ViewBag.Message = "You need to have at least 5 dollars to use CashBack Wallet";
                    return View("Warning");
                }

                var subscription = await _externalContext.InternetProviders.FirstOrDefaultAsync(x => x.SubscriberCode == subscriptionPaymentViewModel.DestinationId && x.AppointmentType == subscriptionPaymentViewModel.AppointmentType);

                if (subscription != null)
                {

                    if (paidByCashBack.Balance < subscriptionPaymentViewModel.Amount)
                    {
                        ViewBag.Message = "Ooo, you ain't got no money to spend !";
                        return View("Warning");
                    }
                    var transactionUtilityTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Internet");
                    var transactionCashBackTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "CashBack");
                    subscriptionPaymentViewModel.Commission = 0;
                    transaction = new()
                    {
                        Id = $"{Guid.NewGuid()}",
                        ReceiptNumber = ReceiptNumberGenerator.GenerateReceiptNumber(),
                        Amount = subscriptionPaymentViewModel.Amount,
                        Commission = subscriptionPaymentViewModel.Commission,
                        BillingAmount = subscriptionPaymentViewModel.Amount + subscriptionPaymentViewModel.Commission,
                        CashbackAmount = 0,
                        TransactionDate = DateTime.UtcNow,
                        PaidByTypeId = transactionCashBackTypeModel.Id,
                        PaidById = subscriptionPaymentViewModel.PaidById,
                        DestinationTypeId = transactionUtilityTypeModel.Id,
                        DestinationId = subscriptionPaymentViewModel.PaidById,
                        StatusId = paidByCashBack.Balance >= subscriptionPaymentViewModel.Amount + subscriptionPaymentViewModel.Commission ? (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Completed")).Id : (await _context.TransactionStatusModels.FirstOrDefaultAsync(x => x.Name == "Failed")).Id,
                        Title = "Internet",
                        Description = subscription.Title,
                    };
                    if (transaction.StatusId == "Completed")
                    {
                        paidByCashBack.Balance -= transaction.BillingAmount;
                        //destinationExternalCard.Balance += transactionViewModel.Amount; // burani bu formada saxlama sebebi bizim amounttun gedib gedmediyi maraqlandirmamasidir 


                    }
                    else if (transaction.StatusId == "Failed")
                    {

                    }




                    await _context.Transactions.AddAsync(transaction);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Transaction", new { id = transaction.Id });



                }
                else
                {
                    ViewBag.Message = "Subscription Not Found";
                    return View("Error");

                }
            }
            else
            {
                ViewBag.Message = "Something went wrong!";
                return View("Error");
            }
        }

        /* End Tranfer Own InternetProviderSubscription */
        /* End Tranfer Own Subscription */

        /* Start Transaction to BakuCard */
        public async Task<IActionResult> BakuCardTransaction()
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
                Models.BankCardType cardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);

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
                BankAccountViewModel = await bankAccount.IsSuspended(_context) || bankAccount.Balance == 0 ? null : bankAccountViewModel
            };
            if (/*transferViewModel.BankCardViewModels.Count == 0 &&*/ transferViewModel.CashBackViewModel == null && transferViewModel.BankAccountViewModel == null)
            {
                ViewBag.Message = "Ooo, you ain't got no money to spend !";
                return View("Warning");
            }


            ViewData["TransferViewModel"] = transferViewModel;

            return View();
        }

        /* End Transaction to BakuCard */









        /*  ----------------------------------------- */


        public async Task<IActionResult> Transaction(string id)
        {
            Transaction transaction = await _context.Transactions.FirstOrDefaultAsync(x => x.Id == id);
            if (transaction == null)
            {
                return BadRequest();
            }
            TransactionDetailsViewModel transactionViewModel = new()
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

            ViewData["TransactionViewModel"] = transactionViewModel;
            TempData["Tab"] = "Payments";

            return View();
        }

        public async Task<IActionResult> RecentActivity()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(user == null)
            {
                return NotFound();
            }

            List<BankCard> bankCards = await _context.BankCards.Where(x => x.UserId == user.Id).ToListAsync();


            BankAccount bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.UserId == user.Id);

            CashBack cashBack = await _context.CashBacks.FirstOrDefaultAsync(x => x.UserId == user.Id);
            

            //start Transaction
            List<Transaction> allTransactions = await _context.Transactions
.AsNoTracking().Where(x =>
                    x.PaidById == cashBack.CashBackNumber ||
                    x.PaidById == bankAccount.AccountNumber||
                    x.DestinationId == bankAccount.AccountNumber
                ).ToListAsync();


            foreach(var userCard in bankCards)
            {
                allTransactions.AddRange( await _context.Transactions.AsNoTracking().Where(x => x.PaidById == userCard.CardNumber || x.DestinationId == userCard.CardNumber).ToListAsync());
            }

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
                    (transaction.PaidById == cashBack.CashBackNumber ||
                    transaction.PaidById == bankAccount.AccountNumber ||
                    bankCards.Any(userCard =>
                        transaction.PaidById == userCard.CardNumber
                    )) &&
                    (bankCards.Any(userCard =>
                        transaction.DestinationId == userCard.CardNumber
                    ) ||
                    transaction.DestinationId == bankAccount.AccountNumber)
                )
                {
                    summary = "Internally";

                }
                else if (
                    (transaction.PaidById == cashBack.CashBackNumber ||
                    transaction.PaidById == bankAccount.AccountNumber ||
                    bankCards.Any(userCard =>
                        transaction.PaidById == userCard.CardNumber
                    )) &&
                    !(bankCards.Any(userCard =>
                        transaction.DestinationId == userCard.CardNumber
                    ) ||
                    transaction.DestinationId == bankAccount.AccountNumber)
                )
                {
                    summary = "Expense";

                }
                else if (
                    !(transaction.PaidById == cashBack.CashBackNumber ||
                    transaction.PaidById == bankAccount.AccountNumber ||
                    bankCards.Any(userCard =>
                        transaction.PaidById == userCard.CardNumber
                    )) &&
                    (bankCards.Any(userCard =>
                        transaction.DestinationId == userCard.CardNumber
                    ) ||
                    transaction.DestinationId == bankAccount.AccountNumber)
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
