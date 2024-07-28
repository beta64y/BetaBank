﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            TempData["Tab"] = "Dashboard";
            return View();

        }
    }
}
