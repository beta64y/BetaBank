using System.ComponentModel.DataAnnotations;

namespace BetaBank.Models
{
    public class CashBack
    {
        public string Id { get; set; }

        public string CashBackNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public double Balance { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
