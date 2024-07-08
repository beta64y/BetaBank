namespace BetaBank.Models
{
    public class BankCardType
    {
        public string Id { get; set; }
        public string CardId { get; set; }
        public BankCard Card { get; set; }
        public string TypeId { get; set; }
        public BankCardTypeModel Type { get; set; }

    }
}
