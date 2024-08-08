using BetaBank.Models;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BetaBank.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;

        public HeaderViewComponent( UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            HeaderViewModel headerViewModel = new();
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                headerViewModel.FirstName = user.FirstName;
                headerViewModel.LastName = user.LastName;
            }
            ViewData["HeaderViewModel"] = headerViewModel;
            return View();
        }
    }
}
