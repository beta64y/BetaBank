using BetaBank.Contexts;
using BetaBank.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.Moderator.ViewComponents
{
    public class ModeratorBoxViewComponent : ViewComponent
    {
        private readonly BetaBankDbContext _context;

        public ModeratorBoxViewComponent(BetaBankDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            TempData["NewsCount"] = await _context.News.Where(x => !x.IsDeleted).CountAsync();
            TempData["SubscribersCount"] = await _context.Subscribers.CountAsync();
            TempData["NotificationMailsCount"] = await _context.SendedNotificationMails.CountAsync();
            return View();
        }
    }
}
