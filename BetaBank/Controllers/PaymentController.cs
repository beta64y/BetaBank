using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace BetaBank.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly BetaBankDbContext _context;

        public PaymentController(BetaBankDbContext context, UserManager<AppUser> userManager)
        {

            _context = context;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Transaction()
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
                bankCardsViewModel.Add(new BankCardViewModel
                {
                    Id = bankCard.Id,
                    Balance = bankCard.Balance,
                    CardNumber = bankCard.CardNumber,
                    
                });
            }
            TransactionViewModel transactionViewModel = new()
            {
                BankCards = bankCardsViewModel,
            };


            return View(transactionViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transaction(TransactionViewModel transferViewModel)
        {
            Console.WriteLine(transferViewModel.CardId);
            Console.WriteLine(transferViewModel.Amount);
            Console.WriteLine(transferViewModel.DestinationCard);


            return Content(transferViewModel.DestinationCard);
        }
    }
}
