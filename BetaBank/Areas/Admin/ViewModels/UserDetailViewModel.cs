namespace BetaBank.Areas.Admin.ViewModels
{
    public class UserDetailViewModel
    {

        public UserBankAccountViewModel? Account { get; set; }
        public List<UserBankCardViewModel> Cards { get; set; }
        public UserCashBackViewModel? CashBack { get; set; }
        public UserViewModel User { get; set; }
    }
}
