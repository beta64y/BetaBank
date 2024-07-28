using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Utils.Enums;
using BetaBank.ViewComponents;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> AddInternetForExternal()
        {
            List<InternetForExternal> internetList = new List<InternetForExternal>
            {
                new InternetForExternal { SubscriberCode = "6437970093017", AppointmentType = "Individuals", Title = "BeeOnline" },
new InternetForExternal { SubscriberCode = "8721489981745", AppointmentType = "Individuals", Title = "KatvInternet" },
new InternetForExternal { SubscriberCode = "5540467233574", AppointmentType = "Individuals", Title = "AzEuroNet" },

                //.... daha cox vardi sadece istifade ettiyini gostermek ucun saxladin

            };

            _externalContext.InternetProviders.AddRange(internetList);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(SubscribeViewModel subscribeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            
            Subscriber subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Mail == subscribeViewModel.Email);
            if (subscriber != null)
            {
                if(subscriber.IsSubscribe)
                {
                    @TempData["SuccessMessage"] = "You are already subscribed!";
                }
                else
                {
                    subscriber.IsSubscribe = true;
                    @TempData["SuccessMessage"] = "You are subscribed!";
                    await _context.SaveChangesAsync();
                }
                return View("Index");

            }
            else
            {
                Subscriber newSubscriber = new()
                {
         
                    Id = $"{Guid.NewGuid()}",
                    Mail = subscribeViewModel.Email,
                    IsSubscribe = true
                };
                await _context.Subscribers.AddAsync(newSubscriber);
                await _context.SaveChangesAsync();
                @TempData["SuccessMessage"] = "You are subscribed!";
            }
            //string refererUrl = Request.Headers["Referer"].ToString();
            // Burani Duzeldecekdin ama sonra erindin 

            return View("Index");
        }


    }
}
