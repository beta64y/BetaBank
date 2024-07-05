namespace BetaBank.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public string IBAN { get; set; } 
        public string SwiftCode { get; set; } 
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedDate { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
