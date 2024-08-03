using BetaBank.Areas.SuperAdmin.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace BetaBank.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    [Authorize(Roles = "SuperAdmin")]
    public class CashBackController : Controller
    {
        private readonly BetaBankDbContext _context;

        public CashBackController(BetaBankDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<CashBack> cashBacks = await _context.CashBacks.AsNoTracking().ToListAsync();
            List<SuperAdmin.ViewModels.CashBackViewModel> walletViewModels = new();
            foreach (CashBack wallet in cashBacks)
            {
                AppUser user = await _context.Users.FirstOrDefaultAsync(x => x.Id ==  wallet.UserId);
                walletViewModels.Add(new SuperAdmin.ViewModels.CashBackViewModel()
                {
                    Id = wallet.Id,
                    Balance = wallet.Balance,
                    CashBackNumber = wallet.CashBackNumber,
                    CreatedDate = wallet.CreatedDate,
                    UpdatedDate = wallet.UpdatedDate,
                    UserId = user.Id,
                    UserFirstName = user.FirstName,
                    UserLastName = user.LastName,
                    UserProfilePhoto = user.ProfilePhoto,
                    
                });
            }
            ViewData["Wallets"] = walletViewModels;

            TempData["Tab"] = "Wallets";
            return View();
        }
    }
}
