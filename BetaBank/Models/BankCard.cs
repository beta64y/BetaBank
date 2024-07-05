using BetaBank.Utils.Enums;
using System.ComponentModel.DataAnnotations;

namespace BetaBank.Models
{
    public class BankCard
    {
        public int Id { get; set; }

        [RegularExpression(@"\d{16}", ErrorMessage = "Card number must be 16 digits.")]
        public string CardNumber { get; set; }
        [RegularExpression(@"\d{3}", ErrorMessage = "CVV must be 3 digits.")]
        public string CVV { get; set; }
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }
        public decimal Balance { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }
 



    }
}
