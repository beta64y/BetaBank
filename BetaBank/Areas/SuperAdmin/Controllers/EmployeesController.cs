using BetaBank.Areas.SuperAdmin.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Services.Implementations;
using BetaBank.Services.Validators;
using BetaBank.Utils.Enums;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace BetaBank.Areas.SuperAdmin.Controllers
{

    [Area("SuperAdmin")]
    [Authorize(Roles = "SuperAdmin")]
    public class EmployeesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly BetaBankDbContext _context;


        public EmployeesController(UserManager<AppUser> userManager, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, RoleManager<IdentityRole> roleManager, BetaBankDbContext context = null)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var roles = new List<string> { "Admin", "Support", "Moderator" };
            var usersInRoles = new List<AppUser>();

            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                usersInRoles.AddRange(usersInRole);
            }

            usersInRoles = usersInRoles.Distinct().ToList();

            var users = usersInRoles
                .AsQueryable()
                .AsNoTracking()
                .OrderBy(b => b.FirstName)
                .ToList();


            List<EmployeeViewModel> usersViewModel = new List<EmployeeViewModel>();
            foreach (var user in users)
            {
                usersViewModel.Add(new EmployeeViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth,
                    PhoneNumber = user.PhoneNumber,
                    CreatedDate = user.CreatedDate,
                    UpdateDate = user.UpdateDate,
                    Banned = user.Banned,
                    ProfilePhoto = user.ProfilePhoto,
                    Email = user.Email,
                    Age = user.DateOfBirth.CalculateAge(),
                    EmailConfirmed = user.EmailConfirmed,
                    Role = (await _userManager.GetRolesAsync(user)).First(),
            });
            }

            ViewData["EmployeeViewModels"] = usersViewModel;
            TempData["Tab"] = "Employees";
            return View();
        }

        public async Task<IActionResult> Create()
        {
            List<IdentityRole> roles = await _context.Roles.ToListAsync();
            List<RoleViewModel> roleViewModels = new List<RoleViewModel>();
            foreach (IdentityRole role in roles)
            {
                if(role.Name == "User" || role.Name == "SuperAdmin")
                {
                    continue;
                }
                roleViewModels.Add(new()
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                });
            }
            TempData["Tab"] = "Employees";
            ViewData["IdentityRoles"] = roleViewModels;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeCreateViewModel employeeCreateViewModel)
        {



            List<IdentityRole> roles = await _context.Roles.ToListAsync();
            List<RoleViewModel> roleViewModels = new List<RoleViewModel>();
            foreach (IdentityRole Role in roles)
            {
                if (Role.Name == "User" || Role.Name == "SuperAdmin")
                {
                    continue;
                }
                roleViewModels.Add(new()
                {
                    RoleId = Role.Id,
                    RoleName = Role.Name,
                });
            }
            TempData["Tab"] = "Employees";
            ViewData["IdentityRoles"] = roleViewModels;
















            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "");
                return View();
            }
            AppUser userByFin = await _userManager.Users.FirstOrDefaultAsync(x => x.FIN == employeeCreateViewModel.FIN);
            
            if (userByFin != null )
            {
                ModelState.AddModelError("", "FIN must be unique");

                return View();
            }
            AppUser userByPhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == employeeCreateViewModel.PhoneNumber);
            if (userByPhoneNumber != null)
            {
                ModelState.AddModelError("", "PhoneNumber must be unique");
                return View();
            }
            if (!employeeCreateViewModel.ProfilePhoto.CheckFileSize(3000))
                {
                    ModelState.AddModelError("Image", "The image is too large, please upload a smaller one.");
                    return View(nameof(Create));
                }

                if (!employeeCreateViewModel.ProfilePhoto.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Image", "The image is too large, please upload a smaller one.");
                    return View(nameof(Create));
                }          
            AppUser appUser = new AppUser()
            {
                UserName = employeeCreateViewModel.Username,
                FIN = employeeCreateViewModel.FIN,
                FirstName = employeeCreateViewModel.FirstName,
                LastName = employeeCreateViewModel.LastName,
                DateOfBirth = employeeCreateViewModel.DateOfBirth,
                PhoneNumber = employeeCreateViewModel.PhoneNumber,
                Biography = employeeCreateViewModel.Biography,
                Email = employeeCreateViewModel.Email,
                CreatedDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                IsActive = true,
                EmailConfirmed = true,



            };

string profilePhotoFileName = await ImageSaverService.SaveImage(employeeCreateViewModel.ProfilePhoto, _webHostEnvironment.WebRootPath, "data");
           appUser.ProfilePhoto = profilePhotoFileName;


            string password = EmployeesService.GeneratePassword();
            IdentityResult identityResult = await _userManager.CreateAsync(appUser, password);
            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {

                    ModelState.AddModelError("", error.Description);
                }

                return View();
            }
            IdentityRole role =  await _context.Roles.FirstOrDefaultAsync(x => x.Id == employeeCreateViewModel.RoleId);
            await _userManager.AddToRoleAsync(appUser, role.Name);



            string path = Path.Combine(_webHostEnvironment.WebRootPath, "templates", "NewEmployeeForAreas.html");
            using StreamReader streamReader = new(path);

            string content = await streamReader.ReadToEndAsync();

            string body = content.Replace("[Employee Name]", $"{appUser.FirstName} {appUser.LastName}");
             body = body.Replace("[Role]", role.Name);
             body = body.Replace("[Password]", password);
             body = body.Replace("[Username]", appUser.UserName);
             body = body.Replace("[Login]", $"https://localhost:7110/{role.Name}/Auth/Login/");

            MailService mailService = new(_configuration);
            await mailService.SendEmailAsync(new MailRequest { ToEmail = appUser.Email, Subject = "Welcome to our family !", Body = body });







           




            return RedirectToAction("Index", "Dashboard");
        }

        public async Task<IActionResult> BanUser(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            user.Banned = true;
            await _context.SaveChangesAsync();

            return Json(new { message = "User has been Banned." });
        }
        public async Task<IActionResult> UnBanUser(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            user.Banned = false;
            await _context.SaveChangesAsync();

            return Json(new { message = "User has been UnBanned." });
        }
        public async Task<IActionResult> Detail(string id)
        {
            var user = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            EmployeeViewModel userViewModel = new EmployeeViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
                CreatedDate = user.CreatedDate,
                UpdateDate = user.UpdateDate,
                Banned = user.Banned,
                ProfilePhoto = user.ProfilePhoto,
                Email = user.Email,
                Age = user.DateOfBirth.CalculateAge(),
                EmailConfirmed = user.EmailConfirmed,
                Role = (await _userManager.GetRolesAsync(user)).First(),
            };
            ViewData["EmployeeViewModels"] = userViewModel;

            List<UserEvent> userEvents = await _context.UserEvents.OrderByDescending(x => x.Date).Where(x => x.UserId == user.Id).ToListAsync();
            List<UserEventViewModel> userEventsViewModel = new List<UserEventViewModel>();

            foreach (var userEvent in userEvents)
            {
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

            return View();
        }
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }


            
            EmployeeUpdateViewModel employeeViewModel = new()
            {
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Biography = user.Biography,
                DateOfBirth = user.DateOfBirth,
                FIN = user.FIN,
                PhoneNumber = user.PhoneNumber,
            };


            TempData["Tab"] = "Employees";
            TempData["Id"] = id;
            return View(employeeViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeUpdateViewModel employeeUpdateViewModel, string id)
        { 
            TempData["Tab"] = "Employees";
            
            
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(nameof(Edit));
            }
            





            if (employeeUpdateViewModel.ProfilePhoto != null)
            {

                if (!employeeUpdateViewModel.ProfilePhoto.CheckFileSize(3000))
                {
                    ModelState.AddModelError("Image", "The image is too large, please upload a smaller one.");
                    return View(nameof(Edit));
                }

                if (!employeeUpdateViewModel.ProfilePhoto.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Image", "The image is too large, please upload a smaller one.");
                    return View(nameof(Edit));
                }
                string basePath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "data");
                string path = Path.Combine(basePath, user.ProfilePhoto);
                if (System.IO.File.Exists(path) && user.ProfilePhoto != "default.png")
                {
                    System.IO.File.Delete(path);
                }
                string profilePhotoFileName = await ImageSaverService.SaveImage(employeeUpdateViewModel.ProfilePhoto, _webHostEnvironment.WebRootPath, "data");
                user.ProfilePhoto = profilePhotoFileName;
            }






            if (user.UserName != employeeUpdateViewModel.Username && _userManager.Users.Any(u => u.UserName == employeeUpdateViewModel.Username))
            {
                ModelState.AddModelError("UserName", "Username Must be unique");
                return View(nameof(Edit));
            }
            if (user.Email != employeeUpdateViewModel.Email && _userManager.Users.Any(u => u.Email == employeeUpdateViewModel.Email))
            {
                ModelState.AddModelError("Email", "Email Must be unique");
                return View(nameof(Edit));
            }
            if (user.FIN != employeeUpdateViewModel.FIN && _userManager.Users.Any(u => u.FIN == employeeUpdateViewModel.FIN))
            {
                ModelState.AddModelError("Email", "FIN Must be unique");
                return View(nameof(Edit));
            }
            if (user.PhoneNumber != employeeUpdateViewModel.PhoneNumber && _userManager.Users.Any(u => u.PhoneNumber == employeeUpdateViewModel.PhoneNumber))
            {
                ModelState.AddModelError("Email", "Phone Number Must be unique");
                return View(nameof(Edit));
            }

            user.FirstName = employeeUpdateViewModel.FirstName;
            user.LastName = employeeUpdateViewModel.LastName;
            user.Biography = employeeUpdateViewModel.Biography;
            user.DateOfBirth = employeeUpdateViewModel.DateOfBirth;
            user.UserName = employeeUpdateViewModel.Username;
            user.FIN = employeeUpdateViewModel.FIN;
            user.Email = employeeUpdateViewModel.Email;
            user.DateOfBirth = employeeUpdateViewModel.DateOfBirth;
            user.UpdateDate = DateTime.UtcNow;

            IdentityResult result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var i in result.Errors)
                {
                    ModelState.AddModelError("", i.Description);
                }
                return View(nameof(Edit));

            }
            return RedirectToAction(nameof(Detail), new { id = id });
            


        }

























        //sekli crop ettikde goturme kodu 
        //[HttpPost]
        //[ValidateAntiForgeryToken]

        //public async Task<IActionResult> Create(ClassViewModel registerViewModel)
        //{
        //    IFormFile file = ImageSaverService.Base64ToIFormFile(registerViewModel.FirstImage, "uploaded_image.png");

        //    // Görüntüyü kaydet
        //    string firstImageFileName = await ImageSaverService.SaveImage(file, _webHostEnvironment.WebRootPath);

        //    return Content($"Image saved successfully as {firstImageFileName}.");



        //}



    }
}
