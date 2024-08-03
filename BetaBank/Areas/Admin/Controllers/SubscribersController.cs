using BetaBank.Areas.Admin.ViewModels;
using BetaBank.Areas.Moderator.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Utils.Enums;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
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
            AdminSubscribersViewModel ViewModel = new AdminSubscribersViewModel()
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
        public async Task<IActionResult> CreateSubscriber()
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
                Action = UserActionType.Get.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.Subscribers.ToString(),
                EntityType = EntityType.Page.ToString(),
                EntityId = "CreateSubscriber"

            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();


            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSubscriber(SubscribeViewModel subscribeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Subscriber subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Mail == subscribeViewModel.Email);
            if (subscriber != null)
            {
                if (subscriber.IsSubscribe)
                {
                    @TempData["SuccessMessage"] = "This User is already subscribed!";
                }
                else
                {
                    subscriber.IsSubscribe = true;
                    TempData["SuccessMessage"] = "User is subscribed!";
                    await _context.SaveChangesAsync();
                }



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








                return RedirectToAction("Index");

            }
            else
            {
                Subscriber newSubscriber = new()
                {

                    Id = $"{Guid.NewGuid()}",
                    Mail = subscribeViewModel.Email,
                    IsSubscribe = true
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
                    Action = UserActionType.Created.ToString(),
                    Date = DateTime.UtcNow,
                    Section = SectionType.Subscribers.ToString(),
                    EntityType = EntityType.Subscriber.ToString(),
                    EntityId = newSubscriber.Id

                };
                await _context.UserEvents.AddAsync(userEvent);


                await _context.Subscribers.AddAsync(newSubscriber);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User is subscribed!";
            }
            //string refererUrl = Request.Headers["Referer"].ToString();
            // Burani Duzeldecekdin ama sonra erindin 

           




            return RedirectToAction("Index");
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

        public async Task<IActionResult> Search(AdminSubscribersViewModel adminSubscribersViewModel)
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
                EntityId = adminSubscribersViewModel.Search.SearchTerm

            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();
            if (adminSubscribersViewModel.Search.SearchTerm != null)
            {
                var searchTerm = adminSubscribersViewModel.Search.SearchTerm.ToLower();
                var filteredSubscribers = await _context.Subscribers.Where(p => (p.Mail.ToLower().Contains(searchTerm))).ToListAsync();
                AdminSubscribersViewModel ViewModel = new AdminSubscribersViewModel()
                {
                    Subscribers = filteredSubscribers,
                    Search = adminSubscribersViewModel.Search
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
