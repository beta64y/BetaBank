using BetaBank.Areas.Moderator.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Utils.Enums;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _userManager;


        public SubscribersController(BetaBankDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            List<Subscriber> subscribers = await _context.Subscribers.AsNoTracking().ToListAsync();
            ModeratorSubscribersViewModel ViewModel = new ModeratorSubscribersViewModel()
            {
                Subscribers = subscribers,
            };
            TempData["Tab"] = "Subscribers";


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
                Section = SectionType.Subscribers.ToString(),
                EntityType = EntityType.Page.ToString(),
                EntityId = "Index"

            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();



            return View(ViewModel);
        }
       
        public async Task<IActionResult> Subscribe(string id)
        {
           
            Subscriber subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Id == id);
            subscriber.IsSubscribe = true;


            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = user.Id,
                Action = UserActionType.MakeSubscribed.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.Subscribers.ToString(),
                EntityType = EntityType.Subscriber.ToString(),
                EntityId = subscriber.Id

            };
            await _context.UserEvents.AddAsync(userEvent);



            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Subscribers");
        }
       
        public async Task<IActionResult> Unsubscribe(string id)
        {

            Subscriber subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Id == id);
            subscriber.IsSubscribe = false;


            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = user.Id,
                Action = UserActionType.MakeUnsubscribed.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.Subscribers.ToString(),
                EntityType = EntityType.Subscriber.ToString(),
                EntityId = subscriber.Id

            };
            await _context.UserEvents.AddAsync(userEvent);




            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Subscribers");
        }

        public async Task<IActionResult> Search(ModeratorSubscribersViewModel moderatorSubscribersViewModel)
        {
            TempData["Tab"] = "Subscribers";

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = user.Id,
                Action = UserActionType.Searched.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.Subscribers.ToString(),
                EntityType = EntityType.Page.ToString(),
                EntityId = moderatorSubscribersViewModel.Search.SearchTerm

            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();
            if (moderatorSubscribersViewModel.Search.SearchTerm != null)
            {
                var searchTerm = moderatorSubscribersViewModel.Search.SearchTerm.ToLower();
                var filteredNews = await _context.Subscribers.Where(p => (p.Mail.ToLower().Contains(searchTerm))).ToListAsync();
                ModeratorSubscribersViewModel ViewModel = new ModeratorSubscribersViewModel()
                {
                    Subscribers = filteredNews,
                    Search = moderatorSubscribersViewModel.Search
                };
                
                return View("Index", ViewModel);
            }
            else
            {
                return View(null);
            }
        }
    }
}
