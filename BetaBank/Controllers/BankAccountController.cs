using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Services.Implementations;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Controllers
{
    [Authorize]
    public class BankAccountController : Controller
    {
        private readonly BetaBankDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BankAccountController(BetaBankDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> CreateBankAccount()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                return NotFound();
            }
            BankAccount bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (bankAccount != null)
            {
                return BadRequest();
            }


                return View();       
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBankAccount(CreateBankAccountViewModel createBankAccountViewModel)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            BankAccount bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (bankAccount != null)
            {
                return BadRequest();
            }
            string accountNumber;
            string iban;
            do
            {
                accountNumber = BankAccountService.GenerateAccountNumber();
                iban = BankAccountService.GenerateIBAN("TR", "00061", accountNumber); 

                var existingAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.IBAN == iban || x.AccountNumber == accountNumber);
                if (existingAccount == null )
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
                UserId = user.Id,
            };
            
            


            var status = await _context.BankAccountStatusModels.FirstOrDefaultAsync(x => x.Name == "UnderReview");
            if (status == null)
            {
                return NotFound();
            }
            var bankAccountStatus = new BankAccountStatus()
            {
                Id = $"{Guid.NewGuid()}",
                AccountId = newbankAccount.Id,
                StatusId = status.Id
            };





            await _context.BankAccounts.AddAsync(newbankAccount);
            await _context.BankAccountStatuses.AddAsync(bankAccountStatus);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");

        }
    }

}
