namespace BetaBank.ViewModels
{
    public class TransactionViewModel
    {
        public List<BankCardViewModel> BankCards { get; set; }
        public string CardId { get; set; }
        public string DestinationCard { get; set; }
        public string Amount {  get; set; }
    }
}
