using System.ComponentModel.DataAnnotations;

namespace BetaBank.ViewModels
{
    public class SubscribeViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
