using BetaBank.Contexts;
using BetaBank.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.ViewComponents
{
    public class NewsCarouselViewComponent : ViewComponent
    {
        private readonly BetaBankDbContext _context;

        public NewsCarouselViewComponent(BetaBankDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<News> news = await _context.News.Where(r => !r.IsDeleted).OrderByDescending(x => x.CreatedDate).ToListAsync();
            ViewData["NewsCarusel"] = news; 
            return View();
        }
    }
}
