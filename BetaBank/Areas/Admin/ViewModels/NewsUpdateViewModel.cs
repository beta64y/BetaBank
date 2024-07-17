using System.ComponentModel.DataAnnotations;

namespace BetaBank.Areas.Admin.ViewModels
{
    public class NewsUpdateViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile FirstImage { get; set; }
        public IFormFile SecondImage { get; set; }
    }
}
