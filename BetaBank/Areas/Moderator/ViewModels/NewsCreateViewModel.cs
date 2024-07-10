using System.ComponentModel.DataAnnotations;

namespace BetaBank.Areas.Moderator.ViewModels
{
    public class NewsCreateViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public IFormFile FirstImage { get; set; }
        [Required]
        public IFormFile SecondImage { get; set; }
    }
}
