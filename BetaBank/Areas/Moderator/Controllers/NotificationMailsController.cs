using BetaBank.Areas.Moderator.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Services.Implementations;
using BetaBank.Utils.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BetaBank.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    [Authorize(Roles = "Moderator")]
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
            List<SendedNotificationMail> notificationMails = await _context.SendedNotificationMails.AsNoTracking().OrderBy(b => b.CreatedDate).ToListAsync();
            ModeratorNotificationMailViewModel ViewModel = new ModeratorNotificationMailViewModel()
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
        public async Task<IActionResult> SendMail(ModeratorCreateNotificationMailViewModel moderatorCreateNotificationMailView, string id)
        {

            MailService mailService = new(_configuration);

            List<Subscriber> subscribers = await _context.Subscribers.Where(x => x.IsSubscribe).ToListAsync();

            foreach (Subscriber subscriber in subscribers)
            {
                await mailService.SendEmailAsync(new BetaBank.ViewModels.MailRequest { ToEmail = subscriber.Mail, Subject = moderatorCreateNotificationMailView.Title, Body = moderatorCreateNotificationMailView.Body });

            }
            SendedNotificationMail sendedNotificationMail = new SendedNotificationMail()
            {
                Id = $"{Guid.NewGuid()}",
                Title = moderatorCreateNotificationMailView.Title,
                Body = moderatorCreateNotificationMailView.Body,
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

            /*string link = Url.Action("ConfirmEmail", "Auth", new { email = appUser.Email, token = token },
                HttpContext.Request.Scheme, HttpContext.Request.Host.Value);
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "templates", "ConfirmEmail.html");
            using StreamReader streamReader = new(path);

            string content = await streamReader.ReadToEndAsync();

            string body = content.Replace("[link]", link);

            MailService mailService = new(_configuration);
            await mailService.SendEmailAsync(new MailRequest { ToEmail = appUser.Email, Subject = "Confirm Email", Body = body });*/
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


        public async Task<IActionResult> Search(ModeratorNotificationMailViewModel NotificationMailViewModel)
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
                EntityId = NotificationMailViewModel.Search.SearchTerm,

            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();





            if (NotificationMailViewModel.Search.SearchTerm != null)
            {
                var searchTerm = NotificationMailViewModel.Search.SearchTerm.ToLower();
                var filteredMails = await _context.SendedNotificationMails.Where(p => (p.Title.ToLower().Contains(searchTerm))).ToListAsync();
                ModeratorNotificationMailViewModel ViewModel = new ModeratorNotificationMailViewModel()
                {
                    NotificationMails = filteredMails,
                    Search = NotificationMailViewModel.Search
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
