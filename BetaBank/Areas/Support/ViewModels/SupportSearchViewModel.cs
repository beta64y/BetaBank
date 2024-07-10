using System.ComponentModel.DataAnnotations;

namespace BetaBank.Areas.Support.ViewModels
{
    public class SupportSearchViewModel
    {
        [Required]
        public string SearchTerm { get; set; }
    }
}
