namespace BetaBank.Models
{
    public class TransactionStatus
    {
        public string Id { get; set; }

        public string TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public string StatusId { get; set; }
        public TransactionStatusModel Status { get; set; }
    }
}
