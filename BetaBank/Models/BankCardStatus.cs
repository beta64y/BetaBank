namespace BetaBank.Models
{
    public class BankCardStatus
    {
        public string Id { get; set; }
        public string CardId { get; set; }
        public BankCard Card { get; set; }
        public string StatusId { get; set; }
        public BankCardStatusModel Status { get; set; }

        
    }
}
