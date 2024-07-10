using BetaBank.Areas.Support.ViewModels;
using BetaBank.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.Support.ViewComponents
{
    public class SupportBoxViewComponent : ViewComponent
    {
        private readonly BetaBankDbContext _context;

        public SupportBoxViewComponent(BetaBankDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var UnderReviewStatus = await _context.SupportStatusModels.FirstOrDefaultAsync(x => x.Name == "UnderReview");
            var PassedStatus = await _context.SupportStatusModels.FirstOrDefaultAsync(x => x.Name == "Passed");
            var AnsweredStatus = await _context.SupportStatusModels.FirstOrDefaultAsync(x => x.Name == "Answered");


            var underReviewCount = await _context.SupportStatuses.Where(x => x.StatusId == UnderReviewStatus.Id).CountAsync();
            var passedCount = await _context.SupportStatuses.Where(x => x.StatusId == PassedStatus.Id).CountAsync();
            var answeredCount = await _context.SupportStatuses.Where(x => x.StatusId == AnsweredStatus.Id).CountAsync();

            SupportBoxViewModel supportBoxViewModel = new()
            {
                AnsweredCount = answeredCount,
                PassedCount = passedCount,
                UnderReviewCount = underReviewCount,
                AnsweredId = AnsweredStatus.Id,
                PassedId = PassedStatus.Id,
                UnderReviewId = UnderReviewStatus.Id
            };

            return View(supportBoxViewModel);
        }
    }
}
