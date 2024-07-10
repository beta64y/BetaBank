using System.ComponentModel.DataAnnotations;

namespace BetaBank.Models
{
    public class Support
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Issue { get; set; }
        public DateTime CreatedDate { get; set; }

        
    }
}
