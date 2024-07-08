using BetaBank.Contexts;
using BetaBank.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Controllers
{
    public class NewsController : Controller
    {
        private readonly BetaBankDbContext _context;

        public NewsController(BetaBankDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> LoadMore(int skip)
        {
            int newsCount = await _context.News.Where(p => !p.IsDeleted).CountAsync();
            if (newsCount <= skip)
            {
                return BadRequest();
            }
            var news = await _context.News.Where(p => !p.IsDeleted).Skip(skip).Take(8).ToListAsync();
            return View("_NewsPartial", news);
        }
        public async Task<IActionResult> NewsDetail(string id)
        {
            var news = await _context.News.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            return View(news);
        }
    }
}
