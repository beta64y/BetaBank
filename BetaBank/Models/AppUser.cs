using Microsoft.AspNetCore.Identity;
using System.Security.Principal;

namespace BetaBank.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }

    }
}
