using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Utils.Enums;
using BetaBank.ViewComponents;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Macs;
using System;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Diagnostics;

namespace BetaBank.Controllers
{
    public class HomeController : Controller
    {
        
        
        
        private readonly BetaBankDbContext _context;
        private readonly ExternalDbContext _externalContext;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(BetaBankDbContext context, IWebHostEnvironment webHostEnvironment, ExternalDbContext externalContext)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _externalContext = externalContext;
        }
        /* Start Create Section */
        public async Task<IActionResult> AddBankCardsForExternal()
        {
            List<BankCardForExternal> bankCardList = new List<BankCardForExternal>
    {
        new BankCardForExternal { Title = "AzerbaijanNationalBank", CardNumber = "5239151711226012" },
        new BankCardForExternal { Title = "AzerbaijanNationalBank", CardNumber = "5239158273645921" },
        new BankCardForExternal { Title = "AzerbaijanNationalBank", CardNumber = "5239159837412564" },
        new BankCardForExternal { Title = "AzerbaijanNationalBank", CardNumber = "5239152654931872" },
        new BankCardForExternal { Title = "AzerbaijanNationalBank", CardNumber = "5239153649274158" },

        new BankCardForExternal { Title = "AzerbaijanTurkishBank", CardNumber = "6348271632547896" },
        new BankCardForExternal { Title = "AzerbaijanTurkishBank", CardNumber = "6348279874152369" },
        new BankCardForExternal { Title = "AzerbaijanTurkishBank", CardNumber = "6348275948172364" },
        new BankCardForExternal { Title = "AzerbaijanTurkishBank", CardNumber = "6348273658942173" },
        new BankCardForExternal { Title = "AzerbaijanTurkishBank", CardNumber = "6348278943261759" },

        new BankCardForExternal { Title = "BirBank", CardNumber = "7894521236894567" },
        new BankCardForExternal { Title = "BirBank", CardNumber = "7894526598741236" },
        new BankCardForExternal { Title = "BirBank", CardNumber = "7894523987654123" },
        new BankCardForExternal { Title = "BirBank", CardNumber = "7894529876543126" },
        new BankCardForExternal { Title = "BirBank", CardNumber = "7894525632147896" },

        new BankCardForExternal { Title = "LeoBank", CardNumber = "4923586471236987" },
        new BankCardForExternal { Title = "LeoBank", CardNumber = "4923581236549874" },
        new BankCardForExternal { Title = "LeoBank", CardNumber = "4923587896541239" },
        new BankCardForExternal { Title = "LeoBank", CardNumber = "4923586547891236" },
        new BankCardForExternal { Title = "LeoBank", CardNumber = "4923583216549871" },

        new BankCardForExternal { Title = "PashaBank", CardNumber = "8745213698542176" },
        new BankCardForExternal { Title = "PashaBank", CardNumber = "8745219635874216" },
        new BankCardForExternal { Title = "PashaBank", CardNumber = "8745216874321985" },
        new BankCardForExternal { Title = "PashaBank", CardNumber = "8745213746985216" },
        new BankCardForExternal { Title = "PashaBank", CardNumber = "8745218653741296" },

        new BankCardForExternal { Title = "XalqBank", CardNumber = "7412589632147895" },
        new BankCardForExternal { Title = "XalqBank", CardNumber = "7412583654128795" },
        new BankCardForExternal { Title = "XalqBank", CardNumber = "7412587894563214" },
        new BankCardForExternal { Title = "XalqBank", CardNumber = "7412589632154879" },
        new BankCardForExternal { Title = "XalqBank", CardNumber = "7412583654789126" },

        new BankCardForExternal { Title = "UniBank", CardNumber = "9632145874123658" },
        new BankCardForExternal { Title = "UniBank", CardNumber = "9632147852369415" },
        new BankCardForExternal { Title = "UniBank", CardNumber = "9632143214569871" },
        new BankCardForExternal { Title = "UniBank", CardNumber = "9632147896543125" },
        new BankCardForExternal { Title = "UniBank", CardNumber = "9632145632147896" }
    };

            _externalContext.BankCards.AddRange(bankCardList);
            _externalContext.SaveChanges();
            return Content("Bank cards added successfully.");
        }

        public async Task<IActionResult> AddPhoneNumbersForExternal()
        {
            List<PhoneNumberForExternal> internetList = new List<PhoneNumberForExternal>
            {

new PhoneNumberForExternal { MobileOperator = "Bakcell", Number = "+994558967013" },
new PhoneNumberForExternal { MobileOperator = "Nar", Number = "+994704050424" },



            };

            _externalContext.PhoneNumbers.AddRange(internetList);
            _externalContext.SaveChanges();
            return Content("sdasda");
        }
        public async Task<IActionResult> AddUsersForExternal()
        {
            List<UsersForExternal> internetList = new List<UsersForExternal>
            {
new UsersForExternal { FIN = "83178RZ" , FirstName = "Məryəm" , LastName ="Cəbrayılova" , FatherName= "Ceyhun" , DateOfBirth = new DateTime(1982, 3, 23) },
 new UsersForExternal { FIN = "7GBHQC9" , FirstName = "İsgəndər" , LastName ="Pənahov" , FatherName= "İlham" , DateOfBirth = new DateTime(1987, 2, 14) },
 new UsersForExternal { FIN = "7S77C0B" , FirstName = "Yəhya" , LastName ="Camalzadə" , FatherName= "Vüqar" , DateOfBirth = new DateTime(2002, 9, 19) },

            };

            _externalContext.Users.AddRange(internetList);
            _externalContext.SaveChanges();
            return Content("sdasda");
        }
        public async Task<IActionResult> AddBakiCartForExternal()
        {
            List<BakuCardForExternal> internetList = new List<BakuCardForExternal>
            {
                new BakuCardForExternal { BakuCardId = "83481-15225-0", Amount = 15.1 },
new BakuCardForExternal { BakuCardId = "36014-00962-9", Amount = 10.1 },
new BakuCardForExternal { BakuCardId = "40889-06435-7", Amount = 9.3 },
            };

            _externalContext.BakuCards.AddRange(internetList);
            _externalContext.SaveChanges();
            return Content("sdasda");
        }
        public async Task<IActionResult> AddUtilityForExternal()
        {
            List<UtilityForExternal> utilitiesList = new List<UtilityForExternal>
            {
                new UtilityForExternal { SubscriberCode = "9663911807108", AppointmentType = "Commercial consumers", Title = "Azeriqaz" },
new UtilityForExternal { SubscriberCode = "5627362014338", AppointmentType = "Individuals", Title = "NakhchivanElektrik" },
new UtilityForExternal { SubscriberCode = "5228525615813", AppointmentType = "Individuals", Title = "AzerIstilikTechizat" },
new UtilityForExternal { SubscriberCode = "3274612216103", AppointmentType = "Individuals", Title = "NakhchivanQaz" },
new UtilityForExternal { SubscriberCode = "2549424323720", AppointmentType = "Commercial consumers", Title = "AzerSu" },
new UtilityForExternal { SubscriberCode = "3624729778039", AppointmentType = "Individuals", Title = "NakhchivanElektrik" },

                //.... daha cox vardi sadece istifade ettiyini gostermek ucun saxladin

            };

            _externalContext.Utilities.AddRange(utilitiesList);
            _externalContext.SaveChanges();
            return Content("sdasda");
        }

        public async Task<IActionResult> CreateEnums()
        {
            //foreach (var items in Enum.GetNames(typeof(Utils.Enums.BankCardStatus)))
            //{
            //    await _context.BankCardStatusModels.AddAsync(new Models.BankCardStatusModel { Id = $"{Guid.NewGuid()}", Name = items });
            //}
            //foreach (var items in Enum.GetNames(typeof(Utils.Enums.BankCardType)))
            //{
            //    await _context.BankCardTypeModels.AddAsync(new Models.BankCardTypeModel { Id = $"{Guid.NewGuid()}", Name = items });
            //}
            //foreach (var items in Enum.GetNames(typeof(Utils.Enums.TransactionStatus)))
            //{
            //    await _context.TransactionStatusModels.AddAsync(new Models.TransactionStatusModel { Id = $"{Guid.NewGuid()}", Name = items });
            //}
            //foreach (var items in Enum.GetNames(typeof(Utils.Enums.TransactionType)))
            //{
            //    await _context.TransactionTypeModels.AddAsync(new Models.TransactionTypeModel { Id = $"{Guid.NewGuid()}", Name = items });
            //}
            //foreach (var items in Enum.GetNames(typeof(Utils.Enums.Utilities)))
            //{
            //    await _context.UtilityModels.AddAsync(new Models.UtilityModel { Id = $"{Guid.NewGuid()}", Name = items, IsAppointmentTypeable = false });
            //}
            //foreach (var items in Enum.GetNames(typeof(Utils.Enums.Internet)))
            //{
            //    await _context.InternetModels.AddAsync(new Models.InternetModel { Id = $"{Guid.NewGuid()}", Name = items, IsAppointmentTypeable = false });
            //}
            //foreach (var items in Enum.GetNames(typeof(Utils.Enums.MobileOperators)))
            //{
            //    await _context.MobileOperatorModels.AddAsync(new Models.MobileOperatorModel { Id = $"{Guid.NewGuid()}", Name = items });
            //}
            //foreach (var items in Enum.GetNames(typeof(Utils.Enums.BankAccountStatus)))
            //{
            //    await _context.BankAccountStatusModels.AddAsync(new Models.BankAccountStatusModel { Id = $"{Guid.NewGuid()}", Name = items });
            //}
            //foreach (var items in Enum.GetNames(typeof(Utils.Enums.SupportStatus)))
            //{
            //    await _context.SupportStatusModels.AddAsync(new Models.SupportStatusModel { Id = $"{Guid.NewGuid()}", Name = items });
            //}
            await _context.SaveChangesAsync();
            return Content("Hamisi yarandi");

        }
        /* End Create Section */

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult FAQ()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Support(SupportViewModel supportViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("FAQ");
            }
            Models.Support support = new()
            {
                Id = $"{Guid.NewGuid()}",
                FirstName = supportViewModel.FirstName ,
                LastName = supportViewModel.LastName ,
                Email = supportViewModel.Email ,
                Issue = supportViewModel.Issue ,
                CreatedDate = DateTime.UtcNow,
            };
            var status = await _context.SupportStatusModels.FirstOrDefaultAsync(x => x.Name == "UnderReview");
            if (status == null)
            {
                return NotFound();
            }
            Models.SupportStatus supportStatus = new()
            {
                Id = $"{Guid.NewGuid()}",
                SupportId = support.Id,
                StatusId = status.Id,
            };

            bool IsMailExists = await _context.Subscribers.Where(x => x.Mail == supportViewModel.Email).AnyAsync();
            if (!IsMailExists)
            {
                Subscriber subscriber = new()
                {
                    Id = $"{Guid.NewGuid()}",
                    Mail = supportViewModel.Email,
                    IsSubscribe = true
                };
                await _context.Subscribers.AddAsync(subscriber);
            }



            await _context.Supports.AddAsync(support);
            await _context.SupportStatuses.AddAsync(supportStatus);
            await _context.SaveChangesAsync();

            @TempData["SuccessMessage"] = "Your Issue has been sent !";
            return RedirectToAction("FAQ","");
        }

        public async Task<IActionResult> Subscribe(string id)
        {
            Subscriber subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Mail == id);
            if (subscriber != null)
            {
                if (subscriber.IsSubscribe)
                {
                    return Json(new { message = "You are already subscribed!" });
                }
                else
                {
                    subscriber.IsSubscribe = true;
                    await _context.SaveChangesAsync();
                    return Json(new { message = "You are subscribed!" });
                }
            }
            else
            {
                Subscriber newSubscriber = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Mail = id,
                    IsSubscribe = true
                };
                await _context.Subscribers.AddAsync(newSubscriber);
                await _context.SaveChangesAsync();
                return Json(new { message = "You are subscribed!" });
            }
        }

    }
}
