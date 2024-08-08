using BetaBank.Areas.Admin.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Services.Implementations;
using BetaBank.Services.Validators;
using BetaBank.Utils.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BetaBank.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NewsController : Controller
    {
        private readonly BetaBankDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NewsController(BetaBankDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<AppUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<News> news = await _context.News.AsNoTracking().OrderByDescending(b => b.CreatedDate).Where(r => !r.IsDeleted).ToListAsync();
            AdminNewsViewModel ViewModel = new AdminNewsViewModel()
            {
                News = news,
            };
            TempData["Tab"] = "News";
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = user.Id,
                Action = UserActionType.Get.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.News.ToString(),
                EntityType = EntityType.Page.ToString(),
                EntityId = "Index"

            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();
            return View(ViewModel);
        }
        public async Task<IActionResult> Create()
        {
            TempData["Tab"] = "News";

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = user.Id,
                Action = UserActionType.Get.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.News.ToString(),
                EntityType = EntityType.Page.ToString(),
                EntityId = "Create"

            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ViewModels.NewsCreateViewModel newsCreateViewModel)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }
            if (!newsCreateViewModel.FirstImage.CheckFileSize(3000))
            {
                ModelState.AddModelError("Image", "The image is too large, please upload a smaller one.");
                return View();
            }
            if (!newsCreateViewModel.FirstImage.CheckFileType("image/"))
            {
                ModelState.AddModelError("Image", "Please upload an image file.");
                return View();
            }
            if (!newsCreateViewModel.SecondImage.CheckFileSize(3000))
            {
                ModelState.AddModelError("Image", "The image is too large, please upload a smaller one.");
                return View();
            }
            if (!newsCreateViewModel.SecondImage.CheckFileType("image/"))
            {
                ModelState.AddModelError("Image", "Please upload an image file.");
                return View();
            }
            string firstImageFileName = await ImageSaverService.SaveImage(newsCreateViewModel.FirstImage, _webHostEnvironment.WebRootPath);
            string secondImageFileName = await ImageSaverService.SaveImage(newsCreateViewModel.SecondImage, _webHostEnvironment.WebRootPath);






            News newNews = new()
            {
                Id = $"{Guid.NewGuid()}",
                Title = newsCreateViewModel.Title,
                Description = newsCreateViewModel.Description,
                FirstImage = firstImageFileName,
                SecondImage = secondImageFileName,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                IsDeleted = false,



            };
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = user.Id,
                Action = UserActionType.Created.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.News.ToString(),
                EntityType = EntityType.News.ToString(),
                EntityId = newNews.Id,

            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.News.AddAsync(newNews);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
       
        public async Task<IActionResult> Delete(string id)
        {
            var news = await _context.News.FirstOrDefaultAsync(r => r.Id == id);
            if (news == null)
            {
                return NotFound();
            }
            news.IsDeleted = true;
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = user.Id,
                Action = UserActionType.Deleted.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.News.ToString(),
                EntityType = EntityType.News.ToString(),
                EntityId = news.Id,

            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();

            return Json(new { message = "Your news has been deleted." });
        }
        public async Task<IActionResult> Detail(string id)
        {
            TempData["Tab"] = "News";
            var news = await _context.News.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = user.Id,
                Action = UserActionType.Viewed.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.News.ToString(),
                EntityType = EntityType.News.ToString(),
                EntityId = news.Id

            };
            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();
            return View(news);
        }


        public async Task<IActionResult> Edit(string id)
        {
            TempData["Tab"] = "News";
            var news = await _context.News.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
            if (news == null)
            {
                return NotFound();
            }

            NewsUpdateViewModel newsUpdateViewModel = new()
            {
                Title = news.Title,
                Description = news.Description,
            };
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = user.Id,
                Action = UserActionType.AttemptedEdit.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.News.ToString(),
                EntityType = EntityType.News.ToString(),
                EntityId = news.Id,

            };

            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();
            return View(newsUpdateViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(NewsUpdateViewModel newsUpdateViewModel, string id)
        {
            var news = await _context.News.FirstOrDefaultAsync(r => r.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            if (newsUpdateViewModel.FirstImage != null)
            {

                if (!newsUpdateViewModel.FirstImage.CheckFileSize(3000))
                {
                    ModelState.AddModelError("Image", "The image is too large, please upload a smaller one.");
                    return View();
                }

                if (!newsUpdateViewModel.FirstImage.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Image", "The image is too large, please upload a smaller one.");
                    return View();
                }
                string basePath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "website-images");
                string path = Path.Combine(basePath, news.FirstImage);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                string firstImageFileName = await ImageSaverService.SaveImage(newsUpdateViewModel.FirstImage, _webHostEnvironment.WebRootPath);
                news.FirstImage = firstImageFileName;
            }
            if (newsUpdateViewModel.SecondImage != null)
            {
                if (!newsUpdateViewModel.SecondImage.CheckFileSize(3000))
                {
                    ModelState.AddModelError("Image", "The image is too large, please upload a smaller one.");
                    return View();
                }

                if (!newsUpdateViewModel.SecondImage.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Image", "The image is too large, please upload a smaller one.");
                    return View();
                }
                string basePath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "website-images");
                string path = Path.Combine(basePath, news.SecondImage);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                string secondImageFileName = await ImageSaverService.SaveImage(newsUpdateViewModel.SecondImage, _webHostEnvironment.WebRootPath);
                news.SecondImage = secondImageFileName;
            }

            news.Title = newsUpdateViewModel.Title;
            news.Description = newsUpdateViewModel.Description;
            news.UpdatedDate = DateTime.UtcNow;

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = user.Id,
                Action = UserActionType.Edited.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.News.ToString(),
                EntityType = EntityType.News.ToString(),
                EntityId = news.Id,

            };
            await _context.UserEvents.AddAsync(userEvent);




            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Search(AdminNewsViewModel adminNewsViewModel)
        {
            TempData["Tab"] = "News";

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            UserEvent userEvent = new()
            {
                Id = $"{Guid.NewGuid()}",
                UserId = user.Id,
                Action = UserActionType.Searched.ToString(),
                Date = DateTime.UtcNow,
                Section = SectionType.News.ToString(),
                EntityType = EntityType.Page.ToString(),
                EntityId = adminNewsViewModel.Search.SearchTerm,

            };

            await _context.UserEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();
            if (adminNewsViewModel.Search.SearchTerm != null)
            {
                var searchTerm = adminNewsViewModel.Search.SearchTerm.ToLower();
                var filteredNews = await _context.News.Where(p => (p.Title.ToLower().Contains(searchTerm) && !p.IsDeleted)).ToListAsync();
                AdminNewsViewModel ViewModel = new AdminNewsViewModel()
                {
                    News = filteredNews,
                    Search = adminNewsViewModel.Search
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
