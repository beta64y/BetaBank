using BetaBank.Contexts;
using BetaBank.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.Moderator.ViewComponents
{
    public class ModeratorNewsViewComponent : ViewComponent
    {
        private readonly BetaBankDbContext _context;

        public ModeratorNewsViewComponent(BetaBankDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<News> news = await _context.News.AsNoTracking().OrderBy(b => b.CreatedDate).Where(r => !r.IsDeleted).ToListAsync();
            return View(news);
        }
    }
}
