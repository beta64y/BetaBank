using BetaBank.Areas.Admin.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PaymentController : Controller
    {
        private readonly BetaBankDbContext _context;

        public PaymentController(BetaBankDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Transaction> transactions = await _context.Transactions.AsNoTracking().ToListAsync();
            ViewData["Transactions"] = transactions;
            TempData["Tab"] = "Users";
            return View();
        }
    }
}
