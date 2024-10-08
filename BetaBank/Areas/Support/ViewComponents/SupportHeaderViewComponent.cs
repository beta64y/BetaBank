﻿using BetaBank.Areas.Support.ViewModels;
using BetaBank.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.Support.ViewComponents
{
    public class SupportHeaderViewComponent : ViewComponent
    {
        private readonly BetaBankDbContext _context;

        public SupportHeaderViewComponent(BetaBankDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var UnderReviewStatus = await _context.SupportStatusModels.FirstOrDefaultAsync(x => x.Name == "UnderReview");
            var PassedStatus = await _context.SupportStatusModels.FirstOrDefaultAsync(x => x.Name == "Passed");
            var AnsweredStatus = await _context.SupportStatusModels.FirstOrDefaultAsync(x => x.Name == "Answered");




            HeaderViewModel headerViewModel = new()
            {

                AnsweredId = AnsweredStatus.Id,
                PassedId = PassedStatus.Id,
                UnderReviewId = UnderReviewStatus.Id
            };
            return View(headerViewModel);
        }
    }
}
