using BetaBank.Areas.Moderator.ViewModels;
using BetaBank.Contexts;
using BetaBank.Models;
using BetaBank.Services.Implementations;
using BetaBank.Services.Validators;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Areas.Moderator.Controllers
{
    [Area("Moderator")]
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
            List<News> news = await _context.News
                .AsNoTracking()
                .OrderBy(b => b.CreatedDate)
                .Where(r => !r.IsDeleted)
                .ToListAsync();
            return View(news);
        }
        public IActionResult Create()
        {
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
                ModelState.AddModelError("Image", "Sekl boyukdu balacasini yukle");
                return View();
            }
            if (!newsCreateViewModel.FirstImage.CheckFileType("image/"))
            {
                ModelState.AddModelError("Image", "sekil gonderde ne pdfni yapisdirmisan");
                return View();
            }
            if (!newsCreateViewModel.SecondImage.CheckFileSize(3000))
            {
                ModelState.AddModelError("Image", "Sekl boyukdu balacasini yukle");
                return View();
            }
            if (!newsCreateViewModel.SecondImage.CheckFileType("image/"))
            {
                ModelState.AddModelError("Image", "sekil gonderde ne pdfni yapisdirmisan");
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
                IsDeleted = false,



            };
            await _context.News.AddAsync(newNews);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        
        
        
        
        
        
        
        
        
        //public async Task<IActionResult> Edit(string id)
        //{

        //    var news = await _context.News.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        //    if (news == null)
        //    {
        //        return NotFound();
        //    }

        //    ProductUpdateViewModel productUpdateViewModel = new()
        //    {
        //        Name = product.Name,
        //        Description = product.Description,
        //        Price = product.Price,
        //        DiscountPercent = product.DiscountPercent,
        //        Rating = product.Rating,

        //        CategoryId = product.CategoryId,
        //    };

        //    ViewBag.Categories = await _context.Categories.AsNoTracking().ToListAsync();

        //    return View(productUpdateViewModel);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ActionName(nameof(Edit))]
        //public async Task<IActionResult> Update(ProductUpdateViewModel productUpdateViewModel, int id)
        //{
        //    if (!productUpdateViewModel.Image.CheckFileSize(3000))
        //    {
        //        ModelState.AddModelError("Image", "get ariqla");
        //        return View();
        //    }

        //    if (!productUpdateViewModel.Image.CheckFileType("image/"))
        //    {
        //        ModelState.AddModelError("Image", "get ariqla");
        //        return View();
        //    }
        //    var product = await _context.Products.FirstOrDefaultAsync(r => r.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    if (productUpdateViewModel.Image != null)
        //    {
        //        string basePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "website-images");
        //        string path = Path.Combine(basePath, product.Image);
        //        if (System.IO.File.Exists(path))
        //        {
        //            System.IO.File.Delete(path);
        //        }
        //        string fileName = $"{Guid.NewGuid()}-{productUpdateViewModel.Image.FileName}";
        //        path = Path.Combine(basePath, fileName);
        //        using (FileStream stream = new(path, FileMode.Create))
        //        {
        //            await productUpdateViewModel.Image.CopyToAsync(stream);
        //        }
        //        product.Image = fileName;
        //    }





        //    product.Name = productUpdateViewModel.Name;
        //    product.Description = productUpdateViewModel.Description;
        //    product.Price = productUpdateViewModel.Price;
        //    product.DiscountPercent = productUpdateViewModel.DiscountPercent;
        //    product.Rating = productUpdateViewModel.Rating;
        //    //product.Image = productUpdateViewModel.Image.FileName;
        //    product.CategoryId = productUpdateViewModel.CategoryId;
        //    product.UpdateDate = DateTime.UtcNow;

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

    }
}
