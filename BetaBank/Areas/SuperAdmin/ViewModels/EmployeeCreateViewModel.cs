using BetaBank.Services.Validators;
using System.ComponentModel.DataAnnotations;

namespace BetaBank.Areas.SuperAdmin.ViewModels
{
    public class EmployeeCreateViewModel
    {

        [Required(ErrorMessage = "Username is required.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username can only contain letters and numbers.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Role ID is required.")]
        public string RoleId { get; set; }

        [Required(ErrorMessage = "Biography is required.")]
        public string Biography { get; set; }

        [Required]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "FIN must be exactly 7 characters long.")]
        [RegularExpression("^[A-Z0-9]{7}$", ErrorMessage = "FIN can only contain uppercase letters (A-Z) and digits (0-9).")]
        public string FIN { get; set; }


        [Required(ErrorMessage = "First name is required.")]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "First name can only contain letters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "Last name can only contain letters.")]
        public string LastName { get; set; }

        [Required]
        [MinAge(18, ErrorMessage = "Yaşınız en az 18 olmalıdır.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        [Required, DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

    
        public IFormFile? ProfilePhoto { get; set; }
    }
}
