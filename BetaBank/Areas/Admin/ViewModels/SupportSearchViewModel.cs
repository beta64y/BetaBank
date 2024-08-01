using System.ComponentModel.DataAnnotations;

namespace BetaBank.Areas.Admin.ViewModels
{
    public class SupportSearchViewModel
    {
        [Required]
        public string SearchTerm { get; set; }
    }
}
