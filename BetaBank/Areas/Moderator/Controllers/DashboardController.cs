using BetaBank.Areas.Moderator.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    [Authorize(Roles = "Moderator")]
    public class DashboardController : Controller
    {
        private readonly BetaBankDbContext _context;

        public DashboardController(BetaBankDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            TempData["Tab"] = "Dashboard";
            ModeratorDashboardViewModel moderatorDashboardViewModel = new ModeratorDashboardViewModel()
            {
                SubscribersCount = await _context.Subscribers.Where(x => x.IsSubscribe).CountAsync(),
                UnsubscribersCount = await _context.Subscribers.Where(x => !x.IsSubscribe).CountAsync(),
              };
            ViewData["ModeratorDashboardViewModel"] = moderatorDashboardViewModel;
            return View();
        }
    }
}
