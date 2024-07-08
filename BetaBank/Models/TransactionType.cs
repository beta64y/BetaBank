namespace BetaBank.Models
{
    public class TransactionType
    {
        public string Id { get; set; }

        public string TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public string TypeId { get; set; }
        public TransactionTypeModel Type { get; set; }
    }
}
