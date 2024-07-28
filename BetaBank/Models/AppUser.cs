using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace BetaBank.Models
{
    public class AppUser : IdentityUser
    {

        public string FIN { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public override string PhoneNumber { get; set; }
        public string ProfilePhoto { get; set; } = "default.png";
        public string? Biography { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }
        
        public bool Banned { get; set; } = false;

    }
}
