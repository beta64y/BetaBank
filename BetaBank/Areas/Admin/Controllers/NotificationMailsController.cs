using BetaBank.Areas.Admin.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
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


        public NotificationMailsController(BetaBankDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            List<SendedNotificationMail> notificationMails = await _context.SendedNotificationMails.AsNoTracking().OrderBy(b => b.CreatedDate).ToListAsync();
            AdminNotificationMailViewModel ViewModel = new AdminNotificationMailViewModel()
            {
                NotificationMails = notificationMails,
            };
            TempData["Tab"] = "NotificationMails";
            return View(ViewModel);
        }
        
        public async Task<IActionResult> SendMail()
        {
            TempData["Tab"] = "NotificationMails";
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
            await _context.SendedNotificationMails.AddAsync(sendedNotificationMail);
            await _context.SaveChangesAsync();
            TempData["Tab"] = "NotificationMails";
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> ViewMail(string id)
        {
            TempData["Tab"] = "NotificationMails";
            var sendedNotificationMail = await _context.SendedNotificationMails.FirstOrDefaultAsync(p => p.Id == id);
            return View(sendedNotificationMail);
        }
        public async Task<IActionResult> Search(AdminNotificationMailViewModel adminNotificationMailViewModel)
        {
            if (adminNotificationMailViewModel.Search.SearchTerm != null)
            {
                var searchTerm = adminNotificationMailViewModel.Search.SearchTerm.ToLower();
                var filteredMails = await _context.SendedNotificationMails.Where(p => (p.Title.ToLower().Contains(searchTerm))).ToListAsync();
                AdminNotificationMailViewModel ViewModel = new AdminNotificationMailViewModel()
                {
                    NotificationMails = filteredMails,
                    Search = adminNotificationMailViewModel.Search
                };
                TempData["Tab"] = "NotificationMails";
                return View("Index", ViewModel);
            }
            else
            {
                TempData["Tab"] = "NotificationMails";
                return View(null);
            }
        }
    }
}
