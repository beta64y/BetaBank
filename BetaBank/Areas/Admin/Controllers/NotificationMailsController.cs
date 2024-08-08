using BetaBank.Areas.Admin.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Services.Implementations;
using BetaBank.Utils.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NotificationMailsController : Controller
    {
        private readonly BetaBankDbContext _context;
        private readonly IConfiguration _configuration;

        private readonly UserManager<AppUser> _userManager;

        public NotificationMailsController(BetaBankDbContext context, IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            List<SendedNotificationMail> notificationMails = await _context.SendedNotificationMails.AsNoTracking().OrderByDescending(b => b.CreatedDate).ToListAsync();
            AdminNotificationMailViewModel ViewModel = new AdminNotificationMailViewModel()
            {
                NotificationMails = notificationMails,
            };
            TempData["Tab"] = "NotificationMails";


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
                Section = SectionType.NotificationMail.ToString(),
                EntityType = EntityType.Page.ToString(),
                EntityId = "Index"

            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();

            return View(ViewModel);
        }
        
        public async Task<IActionResult> SendMail()
        {
            TempData["Tab"] = "NotificationMails";

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
                Section = SectionType.NotificationMail.ToString(),
                EntityType = EntityType.Page.ToString(),
                EntityId = "Send Mail"

            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMail(AdminCreateNotificationMailViewModel adminCreateNotificationMailView)
        {

            MailService mailService = new(_configuration);

            List<Subscriber> subscribers = await _context.Subscribers.Where(x => x.IsSubscribe).ToListAsync();

            foreach (Subscriber subscriber in subscribers)
            {
                await mailService.SendEmailAsync(new BetaBank.ViewModels.MailRequest { ToEmail = subscriber.Mail, Subject = adminCreateNotificationMailView.Title, Body = adminCreateNotificationMailView.Body });

            }
            SendedNotificationMail sendedNotificationMail = new SendedNotificationMail()
            {
                Id = $"{Guid.NewGuid()}",
                Title = adminCreateNotificationMailView.Title,
                Body = adminCreateNotificationMailView.Body,
                CreatedDate = DateTime.UtcNow,
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
                Action = UserActionType.Send.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.NotificationMail.ToString(),
                EntityType = EntityType.NotificationMail.ToString(),
                EntityId = sendedNotificationMail.Id,

            };


            await _context.UserEvents.AddAsync(userEvent);
            await _context.SendedNotificationMails.AddAsync(sendedNotificationMail);
            await _context.SaveChangesAsync();
            TempData["Tab"] = "NotificationMails";
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> ViewMail(string id)
        {
            TempData["Tab"] = "NotificationMails";
            var sendedNotificationMail = await _context.SendedNotificationMails.FirstOrDefaultAsync(p => p.Id == id);
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = user.Id,
                Action = UserActionType.Viewed.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.NotificationMail.ToString(),
                EntityType = EntityType.NotificationMail.ToString(),
                EntityId = sendedNotificationMail.Id

            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();
            return View(sendedNotificationMail);
        }
        public async Task<IActionResult> Search(AdminNotificationMailViewModel adminNotificationMailViewModel)
        {TempData["Tab"] = "NotificationMails";
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
                Section = SectionType.NotificationMail.ToString(),
                EntityType = EntityType.Page.ToString(),
                EntityId = adminNotificationMailViewModel.Search.SearchTerm,

            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();
            if (adminNotificationMailViewModel.Search.SearchTerm != null)
            {
                var searchTerm = adminNotificationMailViewModel.Search.SearchTerm.ToLower();
                var filteredMails = await _context.SendedNotificationMails.Where(p => (p.Title.ToLower().Contains(searchTerm))).ToListAsync();
                AdminNotificationMailViewModel ViewModel = new AdminNotificationMailViewModel()
                {
                    NotificationMails = filteredMails,
                    Search = adminNotificationMailViewModel.Search
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
