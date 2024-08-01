using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.SuperAdmin.ViewComponents
{
    public class SupportSearchViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
