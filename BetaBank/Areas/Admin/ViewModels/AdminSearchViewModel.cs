using System.ComponentModel.DataAnnotations;

namespace BetaBank.Areas.Admin.ViewModels
{
    public class AdminSearchViewModel
    {
        [Required]
        public string SearchTerm { get; set; }
    }
}
