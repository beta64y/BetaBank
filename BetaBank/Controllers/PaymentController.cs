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
        public async Task<IActionResult> TransferAnotherCard()
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
            //TransactionViewModel transactionViewModel = new()
            //{
            //    BankCards = bankCardsViewModel,
            //};
            ViewData["DataList"] = bankCardsViewModel;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransferAnotherCard(TransferCardViewModel transferCardViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "");
                return View();
            }
            BankCard sourceCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == transferCardViewModel.CardNumber);
            if (sourceCard == null)
            {
                return NotFound();  
            }
            BankCard destinationCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == transferCardViewModel.DestinationCardNumber);
            if (sourceCard == null)
            {
                return NotFound();
            }




            return View();
        }
    }
}
