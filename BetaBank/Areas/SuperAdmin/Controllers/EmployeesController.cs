using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.SuperAdmin.Controllers
{
    public class EmployeesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
