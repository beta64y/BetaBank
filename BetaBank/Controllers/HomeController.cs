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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(BetaBankDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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
            foreach (var items in Enum.GetNames(typeof(Utils.Enums.TransactionType)))
            {
                await _context.TransactionTypeModels.AddAsync(new Models.TransactionTypeModel { Id = $"{Guid.NewGuid()}", Name = items });
            }
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
