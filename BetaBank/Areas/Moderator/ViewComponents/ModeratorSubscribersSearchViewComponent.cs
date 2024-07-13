using BetaBank.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.Moderator.ViewComponents
{
    public class ModeratorSubscribersSearchViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
