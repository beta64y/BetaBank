using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.Admin.ViewComponents
{
    public class AdminUserSearchViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
