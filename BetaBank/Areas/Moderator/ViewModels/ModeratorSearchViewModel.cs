using System.ComponentModel.DataAnnotations;

namespace BetaBank.Areas.Moderator.ViewModels
{
    public class ModeratorSearchViewModel
    {
        [Required]
        public string SearchTerm { get; set; }
    }
}
