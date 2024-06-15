using System.ComponentModel.DataAnnotations;

namespace BetaBank.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "FIN must be exactly 7 characters long.")]
        [RegularExpression("^[A-Z0-9]{7}$", ErrorMessage = "FIN can only contain uppercase letters (A-Z) and digits (0-9).")]
        public string FIN { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare(nameof(Password))]
        public string PasswordConfirm { get; set; }
    }
}
