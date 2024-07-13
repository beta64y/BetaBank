using BetaBank.Areas.Moderator.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BetaBank.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    [Authorize(Roles = "Moderator")]
    public class SubscribersController : Controller
    {
        private readonly BetaBankDbContext _context;

        public SubscribersController(BetaBankDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Subscriber> subscribers = await _context.Subscribers.AsNoTracking().ToListAsync();
            ModeratorSubscribersViewModel ViewModel = new ModeratorSubscribersViewModel()
            {
                Subscribers = subscribers,
            };
            TempData["Tab"] = "Subscribers";
            return View(ViewModel);
        }
       
        public async Task<IActionResult> Subscribe(string id)
        {
           
            Subscriber subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Id == id);
            subscriber.IsSubscribe = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Subscribers");
        }
       
        public async Task<IActionResult> Unsubscribe(string id)
        {

            Subscriber subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Id == id);
            subscriber.IsSubscribe = false;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Subscribers");
        }

        public async Task<IActionResult> Search(ModeratorSubscribersViewModel moderatorSubscribersViewModel)
        {
            if (moderatorSubscribersViewModel.Search.SearchTerm != null)
            {
                var searchTerm = moderatorSubscribersViewModel.Search.SearchTerm.ToLower();
                var filteredNews = await _context.Subscribers.Where(p => (p.Mail.ToLower().Contains(searchTerm))).ToListAsync();
                ModeratorSubscribersViewModel ViewModel = new ModeratorSubscribersViewModel()
                {
                    Subscribers = filteredNews,
                    Search = moderatorSubscribersViewModel.Search
                };
                TempData["Tab"] = "Subscribers";
                return View("Index", ViewModel);
            }
            else
            {
                TempData["Tab"] = "Subscribers";
                return View(null);
            }
        }
    }
}
