using System.ComponentModel.DataAnnotations;

namespace BetaBank.Areas.Moderator.ViewModels
{
    public class ModeratorCreateNotificationMailViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
