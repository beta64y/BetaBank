using System.ComponentModel.DataAnnotations;

namespace BetaBank.Areas.SuperAdmin.ViewModels
{
    public class SuperAdminSearchViewModel
    {
        [Required]
        public string SearchTerm { get; set; }
    }
}
