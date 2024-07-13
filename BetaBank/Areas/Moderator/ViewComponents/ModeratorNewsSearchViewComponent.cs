using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.Moderator.ViewComponents
{
    public class ModeratorNewsSearchViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
