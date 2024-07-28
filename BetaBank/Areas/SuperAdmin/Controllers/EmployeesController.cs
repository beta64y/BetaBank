using BetaBank.Areas.SuperAdmin.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Services.Implementations;
using BetaBank.Services.Validators;
using BetaBank.Utils.Enums;
using BetaBank.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Xml.Linq;

namespace BetaBank.Areas.SuperAdmin.Controllers
{

    [Area("SuperAdmin")]
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

        public IActionResult Index()
        {
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
                    ModelState.AddModelError("Image", "get ariqla");
                    return View(nameof(Create));
                }

                if (!employeeCreateViewModel.ProfilePhoto.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Image", "get ariqla");
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



            string path = Path.Combine(_webHostEnvironment.WebRootPath, "templates", "NewEmployeeeForAreas.html");
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
