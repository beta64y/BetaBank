namespace BetaBank.Models
{
    public class BankAccountStatus
    {
        public string Id { get; set; } 
        public string AccountId { get; set; }
        public BankAccount Account { get; set; }
        public string StatusId { get; set; }
        public BankAccountStatusModel Status { get; set; }
    }
}
