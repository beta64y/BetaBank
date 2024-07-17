using BetaBank.Areas.Admin.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
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
            AdminSubscribersViewModel ViewModel = new AdminSubscribersViewModel()
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

        public async Task<IActionResult> Search(AdminSubscribersViewModel adminSubscribersViewModel)
        {
            if (adminSubscribersViewModel.Search.SearchTerm != null)
            {
                var searchTerm = adminSubscribersViewModel.Search.SearchTerm.ToLower();
                var filteredNews = await _context.Subscribers.Where(p => (p.Mail.ToLower().Contains(searchTerm))).ToListAsync();
                AdminSubscribersViewModel ViewModel = new AdminSubscribersViewModel()
                {
                    Subscribers = filteredNews,
                    Search = adminSubscribersViewModel.Search
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
