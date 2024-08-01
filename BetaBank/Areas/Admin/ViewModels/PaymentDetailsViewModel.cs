using BetaBank.Models;

namespace BetaBank.Areas.Admin.ViewModels
{
    public class PaymentDetailsViewModel
    {
        public UserBankAccountViewModel? Account { get; set; }
        public UserBankCardViewModel? Card { get; set; }
        public UserCashBackViewModel? CashBack { get; set; }
        public AppUser User { get; set; }
    }
}
