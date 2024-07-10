using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.Support.ViewComponents
{
    public class SupportSearchViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
