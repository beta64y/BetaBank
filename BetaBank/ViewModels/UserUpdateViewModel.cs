using System.ComponentModel.DataAnnotations;

namespace BetaBank.ViewModels
{
    public class UserUpdateViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required,DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }
        [DataType(DataType.Password),MinLength(8),Compare(nameof(ConfirmPassword))]
        public string? NewPassword { get; set; }
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }





    }
}
