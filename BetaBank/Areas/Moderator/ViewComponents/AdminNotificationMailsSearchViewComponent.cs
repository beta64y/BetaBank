using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.Moderator.ViewComponents
{
    public class NotificationMailsSearchViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
