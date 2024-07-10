using System.ComponentModel.DataAnnotations;

namespace BetaBank.ViewModels
{
    public class SupportViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "First name can only contain letters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "Last name can only contain letters.")]
        public string LastName { get; set; }

        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Issue is required.")]
        [MinLength(10, ErrorMessage = "Issue must be at least 10 characters long.")]
        public string Issue { get; set; }
    }
}
