﻿using BetaBank.Areas.SuperAdmin.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    [Authorize(Roles = "SuperAdmin")]
    public class SubscribersController : Controller
    {
        private readonly BetaBankDbContext _context;

        public SubscribersController(BetaBankDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Subscriber> subscribers = await _context.Subscribers.AsNoTracking().ToListAsync();
            SuperAdminSubscribersViewModel ViewModel = new SuperAdminSubscribersViewModel()
            {
                Subscribers = subscribers,
            };
            TempData["Tab"] = "Subscribers";
            return View(ViewModel);
        }
        public async Task<IActionResult> CreateSubscriber()
        {
            TempData["Tab"] = "Subscribers";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSubscriber(SubscribeViewModel subscribeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Subscriber subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Mail == subscribeViewModel.Email);
            if (subscriber != null)
            {
                if (subscriber.IsSubscribe)
                {
                    @TempData["SuccessMessage"] = "This User is already subscribed!";
                }
                else
                {
                    subscriber.IsSubscribe = true;
                    @TempData["SuccessMessage"] = "User is subscribed!";
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index");

            }
            else
            {
                Subscriber newSubscriber = new()
                {

                    Id = $"{Guid.NewGuid()}",
                    Mail = subscribeViewModel.Email,
                    IsSubscribe = true
                };
                await _context.Subscribers.AddAsync(newSubscriber);
                await _context.SaveChangesAsync();
                @TempData["SuccessMessage"] = "User is subscribed!";
            }
            //string refererUrl = Request.Headers["Referer"].ToString();
            // Burani Duzeldecekdin ama sonra erindin 

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Subscribe(string id)
        {
           
            Subscriber subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Id == id);
            subscriber.IsSubscribe = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Subscribers");
        }
       
        public async Task<IActionResult> Unsubscribe(string id)
        {

            Subscriber subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Id == id);
            subscriber.IsSubscribe = false;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Subscribers");
        }

        public async Task<IActionResult> Search(SuperAdminSubscribersViewModel adminSubscribersViewModel)
        {
            if (adminSubscribersViewModel.Search.SearchTerm != null)
            {
                var searchTerm = adminSubscribersViewModel.Search.SearchTerm.ToLower();
                var filteredSubscribers = await _context.Subscribers.Where(p => (p.Mail.ToLower().Contains(searchTerm))).ToListAsync();
                SuperAdminSubscribersViewModel ViewModel = new SuperAdminSubscribersViewModel()
                {
                    Subscribers = filteredSubscribers,
                    Search = adminSubscribersViewModel.Search
                };
                TempData["Tab"] = "Subscribers";
                return View("Index", ViewModel);
            }
            else
            {
                TempData["Tab"] = "Subscribers";
                return View(null);
            }
        }
    }
}
