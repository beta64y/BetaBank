namespace BetaBank.Models
{
    public class TransactionStatus
    {
        public int Id { get; set; }

        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public int StatusId { get; set; }
        public TransactionStatusModel Status { get; set; }
    }
}
