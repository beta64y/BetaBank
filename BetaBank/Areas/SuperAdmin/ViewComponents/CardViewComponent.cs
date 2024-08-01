using Microsoft.EntityFrameworkCore;
using BetaBank.Areas.SuperAdmin.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.SuperAdmin.ViewComponents
{
    public class CardViewComponent : ViewComponent
    {
        private readonly BetaBankDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CardViewComponent(BetaBankDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var adminCount =  (await _userManager.GetUsersInRoleAsync("Admin")).Count;
            var memberCount = (await _userManager.GetUsersInRoleAsync("User")).Count;
            var moderatorCount = (await _userManager.GetUsersInRoleAsync("Moderator")).Count;
            var supportCount = (await _userManager.GetUsersInRoleAsync("Support")).Count;

            UserCountViewModel userCountViewModel = new()
            {
                AdminCount = adminCount,
                MemberCount = memberCount,
                ModeratorCount = moderatorCount,
                SupportCount = supportCount
            };

            return View(userCountViewModel);
        }
    }
}
