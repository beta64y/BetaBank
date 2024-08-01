using System.ComponentModel.DataAnnotations;

namespace BetaBank.Areas.SuperAdmin.ViewModels
{
    public class SupportSearchViewModel
    {
        [Required]
        public string SearchTerm { get; set; }
    }
}
