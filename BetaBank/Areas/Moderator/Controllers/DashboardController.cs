using BetaBank.Areas.Moderator.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Utils.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    [Authorize(Roles = "Moderator")]
    public class DashboardController : Controller
    {
        private readonly BetaBankDbContext _context;
        private readonly UserManager<AppUser> _userManager;


        public DashboardController(BetaBankDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            TempData["Tab"] = "Dashboard";
            ModeratorDashboardViewModel moderatorDashboardViewModel = new ModeratorDashboardViewModel()
            {
                SubscribersCount = await _context.Subscribers.Where(x => x.IsSubscribe).CountAsync(),
                UnsubscribersCount = await _context.Subscribers.Where(x => !x.IsSubscribe).CountAsync(),
              };
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = user.Id,
                Action = UserActionType.Get.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.Dashboard.ToString(),
                EntityType = EntityType.Page.ToString(),
                EntityId = "Index"
            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();
            
            ViewData["ModeratorDashboardViewModel"] = moderatorDashboardViewModel;
            return View();
        }
    }
}
