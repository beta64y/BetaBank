using System.ComponentModel.DataAnnotations;

namespace BetaBank.Models
{
    public class UserEvent
    {
        public string Id { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        [MaxLength(50)]
        public string Action { get; set; } 

        [MaxLength(50)]
        public string Section { get; set; } 

        public DateTime Date { get; set; }

        [MaxLength(50)]
        public string EntityType { get; set; } 

        public string? EntityId { get; set; }
    }
}
