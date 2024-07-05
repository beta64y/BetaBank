namespace BetaBank.Models
{
    public class BankCardType
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public BankCard Card { get; set; }
        public int TypeId { get; set; }
        public BankCardTypeModel Type { get; set; }

    }
}
