using BetaBank.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.Admin.ViewComponents
{
    public class AdminSubscribersSearchViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
