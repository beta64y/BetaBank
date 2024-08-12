using BetaBank.Areas.SuperAdmin.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    [Authorize(Roles = "SuperAdmin")]
    public class NotificationMailsController : Controller
    {
        private readonly BetaBankDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;



        public NotificationMailsController(BetaBankDbContext context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            List<SendedNotificationMail> notificationMails = await _context.SendedNotificationMails.AsNoTracking().OrderByDescending(b => b.CreatedDate).ToListAsync();
            SuperAdminNotificationMailViewModel ViewModel = new SuperAdminNotificationMailViewModel()
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
        public async Task<IActionResult> SendMail(SuperAdminCreateNotificationMailViewModel adminCreateNotificationMailView)
        {


            List<Subscriber> subscribers = await _context.Subscribers.Where(x => x.IsSubscribe).ToListAsync();



            string path = Path.Combine(_webHostEnvironment.WebRootPath, "templates", "ModeratorMessage.html");
            using StreamReader streamReader = new(path);

            string content = await streamReader.ReadToEndAsync();

            string body = content.Replace("[FirstAndSurName]", $"Subscriber");
            body = body.Replace("[Body]", adminCreateNotificationMailView.Body);
            body = body.Replace("[Subject]", adminCreateNotificationMailView.Title);
            body = body.Replace("[Link]", $"https://localhost:7110/");




            MailService mailService = new(_configuration);



            foreach (Subscriber subscriber in subscribers)
            {
                await mailService.SendEmailAsync(new BetaBank.ViewModels.MailRequest { ToEmail = subscriber.Mail, Subject = adminCreateNotificationMailView.Title, Body = body });
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
        public async Task<IActionResult> Search(SuperAdminNotificationMailViewModel adminNotificationMailViewModel)
        {
            if (adminNotificationMailViewModel.Search.SearchTerm != null)
            {
                var searchTerm = adminNotificationMailViewModel.Search.SearchTerm.ToLower();
                var filteredMails = await _context.SendedNotificationMails.Where(p => (p.Title.ToLower().Contains(searchTerm))).ToListAsync();
                SuperAdminNotificationMailViewModel ViewModel = new SuperAdminNotificationMailViewModel()
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
