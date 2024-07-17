using BetaBank.Contexts;
using BetaBank.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.Moderator.ViewComponents
{
    public class AdminBoxViewComponent : ViewComponent
    {
        private readonly BetaBankDbContext _context;
        private readonly UserManager<AppUser> _userManager;


        public AdminBoxViewComponent(BetaBankDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            TempData["NewsCount"] = await _context.News.Where(x => !x.IsDeleted).CountAsync();
            TempData["SubscribersCount"] = await _context.Subscribers.CountAsync();
            TempData["NotificationMailsCount"] = await _context.SendedNotificationMails.CountAsync();
            TempData["UserCount"] = (await _userManager.GetUsersInRoleAsync("User")).Count;

            return View();
        }
    }
}
