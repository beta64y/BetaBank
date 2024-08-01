using BetaBank.Models;
using BetaBank.Utils.Enums;

namespace BetaBank.Areas.SuperAdmin.ViewModels
{
    public class UserBankAccountViewModel
    {
        public string Id { get; set; }
        public string AccountNumber { get; set; }
        public string IBAN { get; set; }
        public double Balance { get; set; }
        public BankAccountStatusModel AccountStatus { get; set; }
    }
}
