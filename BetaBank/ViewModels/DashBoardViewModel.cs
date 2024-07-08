namespace BetaBank.ViewModels
{
    public class DashBoardViewModel
    {
        public DashBoardUserViewModel User { get; set; }
        public DashBoardBankAccountViewModel BankAccount { get; set; }
        public List<DashBoardBankCardViewModel> BankCards { get; set;}
     }
}
