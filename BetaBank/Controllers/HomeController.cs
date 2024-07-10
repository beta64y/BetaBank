using BetaBank.Contexts;
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

        public HomeController(BetaBankDbContext context)
        {
            _context = context;
        }

        //public async Task<IActionResult> CreateEnums()
        //{
        //    //foreach (var items in Enum.GetNames(typeof(BankCardStatus)))
        //    //{
        //    //    await _context.BankCardStatusModels.AddAsync(new Models.BankCardStatusModel { Id = $"{Guid.NewGuid()}", Name = items });
        //    //}
        //    //foreach (var items in Enum.GetNames(typeof(BankCardType)))
        //    //{
        //    //    await _context.BankCardTypeModels.AddAsync(new Models.BankCardTypeModel { Id = $"{Guid.NewGuid()}", Name = items });
        //    //}
        //    //foreach (var items in Enum.GetNames(typeof(TransactionStatus)))
        //    //{
        //    //    await _context.TransactionStatusModels.AddAsync(new Models.TransactionStatusModel { Id = $"{Guid.NewGuid()}", Name = items });
        //    //}
        //    //foreach (var items in Enum.GetNames(typeof(TransactionType)))
        //    //{
        //    //    await _context.TransactionTypeModels.AddAsync(new Models.TransactionTypeModel { Id = $"{Guid.NewGuid()}", Name = items });
        //    //}
        //    //foreach (var items in Enum.GetNames(typeof(BankAccountStatus)))
        //    //{
        //    //    await _context.BankAccountStatusModels.AddAsync(new Models.BankAccountStatusModel { Id = $"{Guid.NewGuid()}", Name = items });
        //    //}
        //    foreach (var items in Enum.GetNames(typeof(SupportStatus)))
        //    {
        //        await _context.SupportStatusModels.AddAsync(new Models.SupportStatusModel { Id = $"{Guid.NewGuid()}", Name = items });
        //    }
        //    await _context.SaveChangesAsync();
        //    return Content("Hamisi yarandi");

        //}


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



            await _context.Supports.AddAsync(support);
            await _context.SupportStatuses.AddAsync(supportStatus);
            await _context.SaveChangesAsync();

            @TempData["SuccessMessage"] = "Your Issue has been sent !";
            return RedirectToAction("FAQ","");
        } 
    }
}
