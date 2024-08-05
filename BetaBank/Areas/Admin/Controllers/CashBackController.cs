using BetaBank.Areas.Admin.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Utils.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace BetaBank.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CashBackController : Controller
    {
        private readonly BetaBankDbContext _context;
        private readonly UserManager<AppUser> _userManager;


        public CashBackController(BetaBankDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            List<CashBack> cashBacks = await _context.CashBacks.AsNoTracking().ToListAsync();
            List<Admin.ViewModels.CashBackViewModel> walletViewModels = new();
            foreach (CashBack wallet in cashBacks)
            {
                AppUser user = await _context.Users.FirstOrDefaultAsync(x => x.Id ==  wallet.UserId);
                walletViewModels.Add(new Admin.ViewModels.CashBackViewModel()
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

            var employee = await _userManager.FindByNameAsync(User.Identity.Name);
            if (employee == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = employee.Id,
                Action = UserActionType.Get.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.Cards.ToString(),
                EntityType = EntityType.Page.ToString(),
                EntityId = "Index"
            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();

            TempData["Tab"] = "Wallets";
            return View();
        }
    }
}
