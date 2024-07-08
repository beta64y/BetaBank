using BetaBank.Contexts;
using BetaBank.Utils.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Controllers
{
    public class HomeController : Controller
    {
        
        
        
        private readonly BetaBankDbContext _context;

        public HomeController(BetaBankDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CreateEnums()
        {
            foreach (var items in Enum.GetNames(typeof(BankCardStatus)))
            {
                await _context.BankCardStatusModels.AddAsync(new Models.BankCardStatusModel {Id = $"{Guid.NewGuid()}", Name = items });
            }
            foreach (var items in Enum.GetNames(typeof(BankCardType)))
            {
                await _context.BankCardTypeModels.AddAsync(new Models.BankCardTypeModel { Id = $"{Guid.NewGuid()}", Name = items });
            }
            foreach (var items in Enum.GetNames(typeof(TransactionStatus)))
            {
                await _context.TransactionStatusModels.AddAsync(new Models.TransactionStatusModel { Id = $"{Guid.NewGuid()}", Name = items });
            }
            foreach (var items in Enum.GetNames(typeof(TransactionType)))
            {
                await _context.TransactionTypeModels.AddAsync(new Models.TransactionTypeModel { Id = $"{Guid.NewGuid()}", Name = items });
            }
            foreach (var items in Enum.GetNames(typeof(BankAccountStatus)))
            {
                await _context.BankAccountStatusModels.AddAsync(new Models.BankAccountStatusModel { Id = $"{Guid.NewGuid()}", Name = items });
            }
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
    }
}
