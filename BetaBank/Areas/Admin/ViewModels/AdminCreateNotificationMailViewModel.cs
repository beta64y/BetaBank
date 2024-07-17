using System.ComponentModel.DataAnnotations;

namespace BetaBank.Areas.Admin.ViewModels
{
    public class AdminCreateNotificationMailViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
