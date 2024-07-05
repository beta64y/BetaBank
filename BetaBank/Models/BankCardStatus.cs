namespace BetaBank.Models
{
    public class BankCardStatus
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public BankCard Card { get; set; }
        public int StatusId { get; set; }
        public BankCardStatusModel Status { get; set; }

        
    }
}
