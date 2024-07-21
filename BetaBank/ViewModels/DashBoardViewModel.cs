using BetaBank.Models;

namespace BetaBank.ViewModels
{
    public class DashBoardViewModel
    {
        public DashBoardUserViewModel User { get; set; }
        public DashBoardBankAccountViewModel BankAccount { get; set; }
        public List<DashBoardBankCardViewModel> BankCards { get; set;}
        public CashBack CashBack { get; set; }
     }
}
