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
            return View();
        }  

    }
}
