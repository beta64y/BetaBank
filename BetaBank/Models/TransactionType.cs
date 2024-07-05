namespace BetaBank.Models
{
    public class TransactionType
    {
        public int Id { get; set; }

        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public int TypeId { get; set; }
        public TransactionTypeModel Type { get; set; }
    }
}
