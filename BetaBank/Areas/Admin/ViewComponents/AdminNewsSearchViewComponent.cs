using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.Admin.ViewComponents
{
    public class AdminNewsSearchViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
