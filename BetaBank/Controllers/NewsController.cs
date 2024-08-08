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
        public async Task<IActionResult> Index()
        {
            int newsCount = await _context.News.Where(p => !p.IsDeleted).CountAsync();
            ViewBag.NewsCount = newsCount;

            return View();
        }
        public async Task<IActionResult> LoadMore(int skip)
        {
            int newsCount = await _context.News.Where(p => !p.IsDeleted).CountAsync();
            if (newsCount <= skip)
            {
                return BadRequest();
            }
            ViewData["News"] = await _context.News.Where(p => !p.IsDeleted).OrderByDescending(x => x.CreatedDate).Skip(skip).Take(6).ToListAsync();
            return View("_NewsPartial");
        }
        public async Task<IActionResult> Detail(string id)
        {
            var news = await _context.News.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            ViewData["News"] = news;
            return View(news);
        }
    }
}
