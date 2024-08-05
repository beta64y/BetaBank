using BetaBank.Areas.SuperAdmin.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Utils.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    [Authorize(Roles = "SuperAdmin")]
    public class EventController : Controller
    {

        private readonly BetaBankDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public EventController(UserManager<AppUser> userManager, BetaBankDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<UserEvent> userEvents = await _context.UserEvents.OrderByDescending(x => x.Date).ToListAsync();
            List<UserEventViewModel> userEventsViewModel = new List<UserEventViewModel>();

            foreach (var userEvent in userEvents)
            {
                var user = await _userManager.FindByIdAsync(userEvent.UserId);
                UserEventViewModel userEventViewModel = new UserEventViewModel()
                {
                    Action = userEvent.Action,
                    UserId = userEvent.UserId,
                    Section = userEvent.Section,
                    Date = userEvent.Date,
                    EntityId = userEvent.EntityId,
                    UserUsername = user.UserName,
                    UserProfilePhoto = user.ProfilePhoto,
                    EntityType = userEvent.EntityType,
                    Role = (await _userManager.GetRolesAsync(user)).First(),
                };
                if (userEvent.EntityType == EntityType.Page.ToString()) 
                {
                    userEventViewModel.Title = userEvent.EntityId;
                }
                else if (userEvent.EntityType == EntityType.News.ToString())
                {
                    userEventViewModel.Title = (await _context.News.FirstOrDefaultAsync(x => x.Id == userEvent.EntityId)).Title;
                }
                else if (userEvent.EntityType == EntityType.Subscriber.ToString())
                {
                    userEventViewModel.Title = (await _context.Subscribers.FirstOrDefaultAsync(x => x.Id == userEvent.EntityId)).Mail;
                }
                else if (userEvent.EntityType == EntityType.NotificationMail.ToString())
                {
                    userEventViewModel.Title = (await _context.SendedNotificationMails.FirstOrDefaultAsync(x => x.Id == userEvent.EntityId)).Title;

                }
                else if (userEvent.EntityType == EntityType.Support.ToString())
                {
                    BetaBank.Models.Support support = await _context.Supports.FirstOrDefaultAsync(x => x.Id == userEvent.EntityId);
                    userEventViewModel.Title = $"{support.FirstName} {support.LastName}";

                }
                else if (userEvent.EntityType == EntityType.User.ToString())
                {
                    var entityUser = await _userManager.FindByIdAsync(userEvent.EntityId);
                    userEventViewModel.EntityUserFirstName = entityUser.FirstName;
                    userEventViewModel.EntityUserLastName = entityUser.LastName;
                    userEventViewModel.EntityUserProfilePhoto = entityUser.ProfilePhoto;

                }
                else if (userEvent.EntityType == EntityType.Transaction.ToString())
                {
                    userEventViewModel.Title = (await _context.Transactions.FirstOrDefaultAsync(x => x.Id == userEvent.EntityId)).ReceiptNumber;
                }
                else if (userEvent.EntityType == EntityType.BankCard.ToString())
                {
                    BankCard bankCard = await _context.BankCards.FirstOrDefaultAsync(x => x.CardNumber == userEvent.EntityId);

                    Models.BankCardType bankCardType = await _context.BankCardTypes.FirstOrDefaultAsync(x => x.CardId == bankCard.Id);
                    BankCardTypeModel bankCardTypeModel = await _context.BankCardTypeModels.FirstOrDefaultAsync(x => x.Id == bankCardType.TypeId);
                    userEventViewModel.Title = bankCardTypeModel.Name;
                }
                else if (userEvent.EntityType == EntityType.BankAccount.ToString())
                {
                    BankAccount bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(x => x.AccountNumber == userEvent.EntityId);
                    var entityUser = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == bankAccount.UserId);
                    userEventViewModel.EntityUserFirstName = entityUser.FirstName;
                    userEventViewModel.EntityUserLastName = entityUser.LastName;
                    userEventViewModel.EntityUserProfilePhoto = entityUser.ProfilePhoto;
                }
                else if (userEvent.EntityType == EntityType.Wallet.ToString())
                {
                    CashBack cashBack = await _context.CashBacks.FirstOrDefaultAsync(x => x.Id == userEvent.EntityId);
                    var entityUser = await _userManager.FindByIdAsync(cashBack.UserId);
                    userEventViewModel.EntityUserFirstName = entityUser.FirstName;
                    userEventViewModel.EntityUserLastName = entityUser.LastName;
                    userEventViewModel.EntityUserProfilePhoto = entityUser.ProfilePhoto;
                }
                userEventsViewModel.Add(userEventViewModel);
                
            }
            ViewData["UserEventsViewModel"] = userEventsViewModel;
            TempData["Tab"] = "Events";
            return View();
        }
    }
}
