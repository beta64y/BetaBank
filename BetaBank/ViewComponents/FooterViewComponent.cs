using BetaBank.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BetaBank.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
