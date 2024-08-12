using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Services.Implementations;
using BetaBank.Services.Validators;
using BetaBank.Utils.Enums;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BetaBank.Controllers
{
    public class UserController : Controller
    {
        private readonly ExternalDbContext _externalContext;

        private readonly SignInManager<AppUser> _signInManager;
        private readonly BetaBankDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public UserController(UserManager<AppUser> userManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, BetaBankDbContext context, SignInManager<AppUser> signInManager, ExternalDbContext externalContext)
        {
            _userManager = userManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _signInManager = signInManager;
            _externalContext = externalContext;
        }




        public async Task<IActionResult> CheckFIN(string id)
        {
            var userFromDb = await _context.Users.FirstOrDefaultAsync(x => x.FIN == id);
            if (userFromDb != null)
            {
                return Json(new { message = "This FIN already used" });
            }

            var UserFromExternal = await _externalContext.Users.FirstOrDefaultAsync(x => x.FIN == id);
            if (UserFromExternal == null)
            {
                return Json(new { message = "The FIN is invalid." });
            }
            return Json(new { message = "true" });


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
                foreach (var entry in ModelState)
                {
                    var key = entry.Key;
                    var value = entry.Value;

                    foreach (var error in value.Errors)
                    {
                        Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                    }
                }
                return View();
            }
            AppUser userByFin = await _userManager.Users.FirstOrDefaultAsync(x => x.FIN == registerViewModel.FIN);
            if (userByFin != null)
            {
                ViewBag.Message = "Something went wrong!";
                return View("Error");
            }
            AppUser userByPhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == registerViewModel.PhoneNumber);
            if (userByPhoneNumber != null)
            {
                ModelState.AddModelError("", "PhoneNumber must be unique");
                return View();
            }
            var UserFromExternal = await _externalContext.Users.FirstOrDefaultAsync(x => x.FIN == registerViewModel.FIN);
            if (UserFromExternal == null)
            {
                ViewBag.Message = "Something went wrong!";
                return View("Error");
            }
            AppUser appUser = new AppUser()
            {
                UserName = registerViewModel.Email,
                FIN = UserFromExternal.FIN,
                FirstName = UserFromExternal.FirstName,
                LastName = UserFromExternal.LastName,
                DateOfBirth = UserFromExternal.DateOfBirth,
                FatherName = UserFromExternal.FatherName,
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
            string body = content.Replace("[FirstAndSurName]", $"{appUser.FirstName} {appUser.LastName}");

            body = body.Replace("[Link]", link);

            MailService mailService = new(_configuration);
            await mailService.SendEmailAsync(new MailRequest { ToEmail = appUser.Email, Subject = "Confirm Email !", Body = body });







            //subscribe section

            Subscriber subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Mail == registerViewModel.Email);
            if (subscriber != null)
            {
                if (!subscriber.IsSubscribe)
                {
                    subscriber.IsSubscribe = true;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                Subscriber newSubscriber = new()
                {
                    Id = $"{Guid.NewGuid()}",
                    Mail = registerViewModel.Email,
                    IsSubscribe = true
                };
                await _context.Subscribers.AddAsync(newSubscriber);
            }

            //bank account section


            string accountNumber;
            string iban;
            do
            {
                accountNumber = BankAccountService.GenerateAccountNumber();
                iban = BankAccountService.GenerateIBAN("TR", "00061", accountNumber);

                var existingAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.IBAN == iban || x.AccountNumber == accountNumber);
                if (existingAccount == null)
                {
                    break;
                }
            } while (true);

            var swiftCode = BankAccountService.GenerateSWIFT("1234", "AZ");

            var newbankAccount = new BankAccount
            {
                Id = $"{Guid.NewGuid()}",
                AccountNumber = BankAccountService.GenerateAccountNumber(),
                IBAN = iban,
                SwiftCode = swiftCode,
                Balance = 0,
                CreatedDate = DateTime.UtcNow,
                UserId = appUser.Id,
            };




            var status = await _context.BankAccountStatusModels.FirstOrDefaultAsync(x => x.Name == "Active");
            if (status == null)
            {
                return NotFound();
            }
            var bankAccountStatus = new Models.BankAccountStatus()
            {
                Id = $"{Guid.NewGuid()}",
                AccountId = newbankAccount.Id,
                StatusId = status.Id
            };

            //cashback section
            string cashBackNumber;
            do
            {
                cashBackNumber = CashBackService.GenerateNumber();

                var existingCashBack = await _context.CashBacks.FirstOrDefaultAsync(x => x.CashBackNumber == cashBackNumber);
                if (existingCashBack == null)
                {
                    break;
                }
            } while (true);
            CashBack newCashBack = new()
            {
                Id = $"{Guid.NewGuid()}",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                UserId = appUser.Id,
                Balance = 0,
                CashBackNumber = cashBackNumber,
            };


            await _context.CashBacks.AddAsync(newCashBack);


            await _context.BankAccounts.AddAsync(newbankAccount);
            await _context.BankAccountStatuses.AddAsync(bankAccountStatus);


            await _context.SaveChangesAsync();
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
            //start BankCards
            List<BankCard> bankCards = await _context.BankCards.Where(x => x.UserId == user.Id).ToListAsync();

            List<BankCardDetailsViewModel> bankCardViewModels = new();
            if (bankCards != null)
            {
                foreach (var bankCard in bankCards)
                {
                    Models.BankCardStatus cardStatus = await _context.BankCardStatuses.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);
                    Models.BankCardType cardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);

                    bankCardViewModels.Add(new BankCardDetailsViewModel()
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

                    });
                }
            }
            ViewData["Cards"] = bankCardViewModels;
            //end BankCards
            //start BankAccount
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


            //end BankAccount
            //start CasBack
            CashBack cashBack = await _context.CashBacks.FirstOrDefaultAsync(x => x.UserId == user.Id);
            CashBackDetailsViewModel wallet = new()
            {
                Id = cashBack.Id,
                Balance = cashBack.Balance,
                CreatedDate = cashBack.CreatedDate,
                UpdatedDate = cashBack.UpdatedDate,
                CashBackNumber = cashBack.CashBackNumber,};
            ViewData["CashBack"] = wallet;

            //end Cashback
            //start Transaction
            List<Transaction> allTransactions = await _context.Transactions
.AsNoTracking().Where(x =>
                    x.PaidById == wallet.CashBackNumber ||
                    x.PaidById == bankAccountViewModel.AccountNumber||
                    x.DestinationId == bankAccountViewModel.AccountNumber
                ).ToListAsync();


            foreach(var userCard in bankCardViewModels)
            {
                allTransactions.AddRange( await _context.Transactions.AsNoTracking().Where(x => x.PaidById == userCard.CardNumber || x.DestinationId == userCard.CardNumber).ToListAsync());
            }

            List<TransactionDetailsViewModel> transactionViewModels = new();

            foreach (Transaction transaction in allTransactions)
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
                    if(card != null)
                    {
                        destinationCardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == card.Id);
                    }
                    
                    

                }
                string summary = null;
                if (
                    (transaction.PaidById == wallet.CashBackNumber ||
                    transaction.PaidById == bankAccountViewModel.AccountNumber ||
                    bankCardViewModels.Any(userCard =>
                        transaction.PaidById == userCard.CardNumber
                    )) &&
                    (bankCardViewModels.Any(userCard =>
                        transaction.DestinationId == userCard.CardNumber
                    ) ||
                    transaction.DestinationId == bankAccountViewModel.AccountNumber)
                )
                {
                    summary = "Internally";

                }
                else if (
                    (transaction.PaidById == wallet.CashBackNumber ||
                    transaction.PaidById == bankAccountViewModel.AccountNumber ||
                    bankCardViewModels.Any(userCard =>
                        transaction.PaidById == userCard.CardNumber
                    )) &&
                    !(bankCardViewModels.Any(userCard =>
                        transaction.DestinationId == userCard.CardNumber
                    ) ||
                    transaction.DestinationId == bankAccountViewModel.AccountNumber)
                )
                {
                    summary = "Expense";

                }
                else if (
                    !(transaction.PaidById == wallet.CashBackNumber ||
                    transaction.PaidById == bankAccountViewModel.AccountNumber ||
                    bankCardViewModels.Any(userCard =>
                        transaction.PaidById == userCard.CardNumber
                    )) &&
                    (bankCardViewModels.Any(userCard =>
                        transaction.DestinationId == userCard.CardNumber
                    ) ||
                    transaction.DestinationId == bankAccountViewModel.AccountNumber)
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

            var lastTwoTransactions = transactionViewModels.OrderByDescending(t => t.TransactionDate).Take(2).ToList();

            // Get transactions where the destination type is "Card".
            var cardTransactions = transactionViewModels
                .Where(t => t.DestinationType.Name == "Card")
                .OrderByDescending(t => t.TransactionDate) // Optional: Order by date if needed
                .Take(2).ToList();

            // Add to ViewData
            ViewData["LastTwoTransactions"] = lastTwoTransactions;
            ViewData["CardTransactions"] = cardTransactions;


            ViewData["Transactions"] = transactionViewModels;
            //end Transaction


            //subscriptions
            TransactionTypeModel utilityTransactionTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Utility");
            TransactionTypeModel internetProviderTransactionTypeModel = await _context.TransactionTypeModels.FirstOrDefaultAsync(x => x.Name == "Internet");


            ViewData["UtilityTransactionTypeModel"] = utilityTransactionTypeModel;
            ViewData["InternetProviderTransactionTypeModel"] = internetProviderTransactionTypeModel;



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





            return View();
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
            TempData["ProfilePhoto"] = user.ProfilePhoto;
            ViewData["UserProfileViewmodel"] = userProfileViewModel;



            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserUpdateViewModel userUpdateViewModel)
        {
            TempData["Tab"] = "account-details";
            
            UserProfileViewmodel userProfileViewmodel = new()
            {
                userUpdateViewModel = userUpdateViewModel
            };

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            TempData["ProfilePhoto"] = user.ProfilePhoto;
            ViewData["UserProfileViewmodel"] = userProfileViewmodel;
            if (!ModelState.IsValid)
            {
                return View(nameof(Profile));
            }

            




            if (userUpdateViewModel.ProfilePhoto != null)
            {

                if (!userUpdateViewModel.ProfilePhoto.CheckFileSize(3000))
                {
                    ModelState.AddModelError("Image", "The image is too large, please upload a smaller one.");
                    return View(nameof(Profile));
                }

                if (!userUpdateViewModel.ProfilePhoto.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Image", "The image is too large, please upload a smaller one.");
                    return View(nameof(Profile));
                }
                string basePath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "data");
                string path = Path.Combine(basePath, user.ProfilePhoto);
                if (System.IO.File.Exists(path) && user.ProfilePhoto !=  "default.png")
                {
                    System.IO.File.Delete(path);
                }
                string profilePhotoFileName = await ImageSaverService.SaveImage(userUpdateViewModel.ProfilePhoto, _webHostEnvironment.WebRootPath,"data");
                user.ProfilePhoto = profilePhotoFileName;
                TempData["ProfilePhoto"] = profilePhotoFileName;
            }






            if (user.UserName != userUpdateViewModel.Email && _userManager.Users.Any(u => u.UserName == userUpdateViewModel.Email))
            {
                ModelState.AddModelError("UserName", "UserName Must be unique");
                return View(nameof(Profile));
            }
            if (user.Email != userUpdateViewModel.Email && _userManager.Users.Any(u => u.Email == userUpdateViewModel.Email))
            {
                ModelState.AddModelError("Email", "Email Must be unique");
                return View(nameof(Profile));
            }

            if (userUpdateViewModel.CurrentPassword != null)
            {
                if (userUpdateViewModel.NewPassword == null)
                {
                    ModelState.AddModelError("NewPassword", "New password cannot be empty.");
                    return View(nameof(Profile));

                }
                IdentityResult identityResult = await _userManager.ChangePasswordAsync(user, userUpdateViewModel.CurrentPassword, userUpdateViewModel.NewPassword);
                if (!identityResult.Succeeded)
                {
                    foreach (var i in identityResult.Errors)
                    {
                        ModelState.AddModelError("", i.Description);
                    }
                    return View(nameof(Profile));
                }
            }

        
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
                return View(nameof(Profile));

            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Sizin profiliniz ugurla yenilendi";
            TempData["ProfilePhoto"] = user.ProfilePhoto;
            return View(nameof(Profile));


        }


    }
}
