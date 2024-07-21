namespace BetaBank.Models
{
    public class TransactionStatus
    {
        public string Id { get; set; }
        public string SupportId { get; set; }
        public Transaction Support { get; set; }
        public string TransactionId { get; set; }
        public TransactionStatusModel Transaction { get; set; }
    }
}
