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
            List<News> news = await _context.News.AsNoTracking().OrderBy(b => b.CreatedDate).Where(r => !r.IsDeleted).ToListAsync();
            TempData["Tab"] = "Dashboard";
            return View(news);
        }
    }
}
