using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Utils.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashBoardController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
private readonly BetaBankDbContext _context;
        public DashBoardController(UserManager<AppUser> userManager, BetaBankDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        public async Task<IActionResult> Index()
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
                Section = SectionType.Dashboard.ToString(),
                EntityType = EntityType.Page.ToString(),
                EntityId = "Index"
            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();


            TempData["Tab"] = "Dashboard";
            return View();

        }
    }
}
