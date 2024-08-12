using BetaBank.Areas.SuperAdmin.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    [Authorize(Roles = "SuperAdmin")]
    public class SupportsController : Controller
    {
        
        private readonly BetaBankDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SupportsController(BetaBankDbContext context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            List<Models.Support> supports = await _context.Supports.ToListAsync();
            List<SupportViewModel> supportsViewModel = new List<SupportViewModel>();
            foreach (var support in supports)
            {
                SupportStatus supportStatus = await _context.SupportStatuses.FirstOrDefaultAsync(x => x.SupportId == support.Id);
                supportsViewModel.Add(new SupportViewModel()
                {
                    Id = support.Id,
                    FirstName = support.FirstName,
                    LastName = support.LastName,
                    Email = support.Email,
                    Issue = support.Issue,
                    CreatedDate = support.CreatedDate,
                    Status = await _context.SupportStatusModels.FirstOrDefaultAsync(x => x.Id == supportStatus.StatusId)

                });
            }
            SupportComponentsViewModel supportComponentsViewModel = new()
            {
                Supports = supportsViewModel,
            };
            TempData["Tab"] = "Supports";
            return View(supportComponentsViewModel);
        }

        //public async Task<IActionResult> FilteredSupports(string id)
        //{
        //    List<Models.SupportStatus> statuses = await _context.SupportStatuses.Where(x => x.StatusId == id).ToListAsync();
        //    List<SupportViewModel> supportsViewModel = new List<SupportViewModel>();
        //    SupportStatusModel supportStatusModel = await _context.SupportStatusModels.FirstOrDefaultAsync(x => x.Id == id);
        //    foreach (var status in statuses)
        //    {
        //        Models.Support support = await _context.Supports.FirstOrDefaultAsync(x => x.Id == status.SupportId);

        //        supportsViewModel.Add(new SupportViewModel()
        //        {
        //            Id = support.Id,
        //            FirstName = support.FirstName,
        //            LastName = support.LastName,
        //            Email = support.Email,
        //            Issue = support.Issue,
        //            CreatedDate = support.CreatedDate,
        //            Status = supportStatusModel

        //        });
        //    }
        //    SupportComponentsViewModel supportComponentsViewModel = new()
        //    {
        //        Supports = supportsViewModel,

        //    };
        //    TempData["Tab"] = supportStatusModel.Name;
        //    return View(supportComponentsViewModel);
        //}
        public async Task<IActionResult> ManageSupport(string id)
        {
            Models.Support support = await _context.Supports.FirstOrDefaultAsync(x => x.Id == id);
            SupportStatus supportStatus = await _context.SupportStatuses.FirstOrDefaultAsync(x => x.SupportId == support.Id);
            SupportStatusModel supportStatusModel = await _context.SupportStatusModels.FirstOrDefaultAsync(x => x.Id == supportStatus.StatusId);


            TempData["Id"] = support.Id;
            TempData["FirstName"] = support.FirstName;
            TempData["LastName"] = support.LastName;
            TempData["Email"] = support.Email;
            TempData["Issue"] = support.Issue;
            TempData["CreatedDate"] = support.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");
            TempData["StatusName"] = supportStatusModel.Name;
            TempData["Tab"] = "Supports";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PassSupport(AnswerSupportViewModel answerSupportViewModel, string id)
        {
            SupportStatus supportStatus = await _context.SupportStatuses.FirstOrDefaultAsync(x => x.SupportId == id);
            SupportStatusModel supportStatusModel = await _context.SupportStatusModels.FirstOrDefaultAsync(x => x.Name == "Passed");
            supportStatus.StatusId = supportStatusModel.Id;
            await _context.SaveChangesAsync();
            TempData["Tab"] = "Supports";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnswerSupport(AnswerSupportViewModel answerSupportViewModel, string id)
        {


            Models.Support support = await _context.Supports.FirstOrDefaultAsync(x => x.Id == id);



            string path = Path.Combine(_webHostEnvironment.WebRootPath, "templates", "SupportMessage.html");
            using StreamReader streamReader = new(path);

            string content = await streamReader.ReadToEndAsync();

            string body = content.Replace("[FirstAndSurName]", $"{support.FirstName} {support.LastName}");
            body = body.Replace("[Body]", answerSupportViewModel.Body);
            body = body.Replace("[Subject]", answerSupportViewModel.Title);
            body = body.Replace("[Link]", $"https://localhost:7110/");




            MailService mailService = new(_configuration);
            await mailService.SendEmailAsync(new BetaBank.ViewModels.MailRequest { ToEmail = answerSupportViewModel.Mail, Subject = answerSupportViewModel.Title, Body = body });






            SupportStatus supportStatus = await _context.SupportStatuses.FirstOrDefaultAsync(x => x.SupportId == id);
            SupportStatusModel supportStatusModel = await _context.SupportStatusModels.FirstOrDefaultAsync(x => x.Name == "Answered");

            supportStatus.StatusId = supportStatusModel.Id;

            await _context.SaveChangesAsync();
            TempData["Tab"] = "Supports";
            return RedirectToAction(nameof(Index));

            /*string link = Url.Action("ConfirmEmail", "Auth", new { email = appUser.Email, token = token },
                HttpContext.Request.Scheme, HttpContext.Request.Host.Value);
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "templates", "ConfirmEmail.html");
            using StreamReader streamReader = new(path);

            string content = await streamReader.ReadToEndAsync();

            string body = content.Replace("[link]", link);

            MailService mailService = new(_configuration);
            await mailService.SendEmailAsync(new MailRequest { ToEmail = appUser.Email, Subject = "Confirm Email", Body = body });*/
        }
        [HttpPost]
        public async Task<IActionResult> Search(SupportComponentsViewModel supportComponentViewModel)
        {
            if (supportComponentViewModel.SupportSearch.SearchTerm != null)
            {
                TempData["Tab"] = "Supports";
                var searchTerm = supportComponentViewModel.SupportSearch.SearchTerm.ToLower();
                var filteredSupports = await _context.Supports.Where(p => (p.Email.ToLower().Contains(searchTerm) || p.FirstName.ToLower().Contains(searchTerm) || p.LastName.ToLower().Contains(searchTerm))).ToListAsync();
                List<SupportViewModel> supportViewModels = new();
                foreach (var support in filteredSupports)
                {
                    SupportStatus supportStatus = await _context.SupportStatuses.FirstOrDefaultAsync(x => x.SupportId == support.Id);
                    supportViewModels.Add(new SupportViewModel()
                    {
                        Email = support.Email,
                        Id = support.Id,
                        FirstName = support.FirstName,
                        LastName = support.LastName,
                        CreatedDate = DateTime.Now,
                        Status = await _context.SupportStatusModels.FirstOrDefaultAsync(x => x.Id == supportStatus.StatusId)
                    });
                }
                SupportComponentsViewModel ViewModel = new SupportComponentsViewModel()
                {
                    Supports = supportViewModels,
                    SupportSearch = supportComponentViewModel.SupportSearch
                };
                return View("Index", ViewModel);
            }
            else
            {
                return View(null);
            }
        }
    }
}
