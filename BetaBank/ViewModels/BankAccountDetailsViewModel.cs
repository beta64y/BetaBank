using BetaBank.Models;

namespace BetaBank.ViewModels
{
    public class BankAccountDetailsViewModel
    {
        public string Id { get; set; }
        public string AccountNumber { get; set; }
        public string IBAN { get; set; }
        public double Balance { get; set; }
        public BankAccountStatusModel AccountStatus { get; set; }
    }
}
