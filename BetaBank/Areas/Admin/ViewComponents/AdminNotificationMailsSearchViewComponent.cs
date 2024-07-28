using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.Admin.ViewComponents
{
    public class AdminNotificationMailsSearchViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
