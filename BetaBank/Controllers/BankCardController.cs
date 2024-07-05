using BetaBank.Contexts;
using BetaBank;
using BetaBank.Models;
using BetaBank.ViewModels;
using BetaBank.Services.Implementations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BetaBank.Controllers
{
    //[Authorize]
    public class BankCardController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly BetaBankDbContext _context;

        public BankCardController(BetaBankDbContext context, UserManager<AppUser> userManager)
        {

            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> GetCard()
        {
            ViewBag.BankCardTypes = await _context.BankCardTypeModels.AsNoTracking().ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetCard(GetBankCardViewModel getBankCardViewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.BankCardTypes = await _context.BankCardTypeModels.AsNoTracking().ToListAsync();
                return View();
            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            var bankCard = new BankCard()
            {
                CardNumber = BankCardService.GenerateCardNumber(),
                CVV = BankCardService.GenerateCVV(),
                ExpiryDate = BankCardService.GenerateExpiryDate(),
                Balance = 0,
                UserId = user.Id,
            };

            await _context.BankCards.AddAsync(bankCard);
            await _context.SaveChangesAsync();



            var status = await _context.BankCardStatusModels.FirstOrDefaultAsync(x => x.Name == "UnderReview");
            if(status == null)
            {
                return NotFound();
            }
            var bankCardStatus = new BankCardStatus()
            {
                CardId = bankCard.Id,
                StatusId = status.Id

            };

            var type = await _context.BankCardTypeModels.FirstOrDefaultAsync(x => x.Id == getBankCardViewModel.TypeId);
            if (type == null)
            {
                return NotFound();
            }
            var bankCardType = new BankCardType()
            {
                CardId = bankCard.Id,
                TypeId = type.Id
            };


            
            await _context.BankCardStatuses.AddAsync(bankCardStatus);
            await _context.BankCardTypes.AddAsync(bankCardType);
            await _context.SaveChangesAsync();

            return  RedirectToAction("Index", "Home");
        }






    }
}
