using BetaBank.Areas.Support.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;

namespace BetaBank.Areas.Support.Controllers
{
    [Area("Support")]
    //[Authorize(Roles = "Support")]
    public class DashboardController : Controller
    {
        private readonly BetaBankDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DashboardController(BetaBankDbContext context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }


        public async  Task<IActionResult> Index()
        {
            var UnderReviewStatus = await _context.SupportStatusModels.FirstOrDefaultAsync(x => x.Name == "UnderReview");
            var PassedStatus = await _context.SupportStatusModels.FirstOrDefaultAsync(x => x.Name == "Passed");
            var AnsweredStatus = await _context.SupportStatusModels.FirstOrDefaultAsync(x => x.Name == "Answered");


            var underReviewCount = await _context.SupportStatuses.Where(x => x.StatusId == UnderReviewStatus.Id).CountAsync();
            var passedCount = await _context.SupportStatuses.Where(x => x.StatusId == PassedStatus.Id).CountAsync();
            var answeredCount = await _context.SupportStatuses.Where(x => x.StatusId == AnsweredStatus.Id).CountAsync();
            var supportCount = await _context.Supports.CountAsync();

            SupportBoxViewModel supportBoxViewModel = new()
            {
                SupportCount = supportCount,
                AnsweredCount = answeredCount,
                PassedCount = passedCount,
                UnderReviewCount = underReviewCount,
                AnsweredId = AnsweredStatus.Id,
                PassedId = PassedStatus.Id,
                UnderReviewId = UnderReviewStatus.Id
            };
            ViewData["SupportBoxViewModel"] = supportBoxViewModel;
            return View();
        }  

    }
}
