using BetaBank.Areas.SuperAdmin.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Services.Implementations;
using BetaBank.Services.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BetaBank.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    [Authorize(Roles = "SuperAdmin")]
    public class NewsController : Controller
    {
        private readonly BetaBankDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NewsController(BetaBankDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            List<News> news = await _context.News.AsNoTracking().OrderBy(b => b.CreatedDate).Where(r => !r.IsDeleted).ToListAsync();
            SuperAdminNewsViewModel ViewModel = new SuperAdminNewsViewModel()
            {
                News = news,
            };
            TempData["Tab"] = "News";
            return View(ViewModel);
        }
        public IActionResult Create()
        {
            TempData["Tab"] = "News";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsCreateViewModel newsCreateViewModel)
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
            await _context.SaveChangesAsync();

            return Json(new { message = "Your news has been deleted." });
        }
        public async Task<IActionResult> Detail(string id)
        {
            TempData["Tab"] = "News";
            var news = await _context.News.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
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

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Search(SuperAdminNewsViewModel adminNewsViewModel)
        {
            if (adminNewsViewModel.Search.SearchTerm != null)
            {
                var searchTerm = adminNewsViewModel.Search.SearchTerm.ToLower();
                var filteredNews = await _context.News.Where(p => (p.Title.ToLower().Contains(searchTerm) && !p.IsDeleted)).ToListAsync();
                SuperAdminNewsViewModel ViewModel = new SuperAdminNewsViewModel()
                {
                    News = filteredNews,
                    Search = adminNewsViewModel.Search
                };
                TempData["Tab"] = "News";

                return View("Index", ViewModel);
            }
            else
            {
                TempData["Tab"] = "News";

                return View(null);
            }
        }
    }
}
