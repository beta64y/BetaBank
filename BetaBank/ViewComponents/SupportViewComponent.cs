using BetaBank.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace BetaBank.ViewComponents
{
    public class SupportViewComponent : ViewComponent
    {
        private readonly BetaBankDbContext _context;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
