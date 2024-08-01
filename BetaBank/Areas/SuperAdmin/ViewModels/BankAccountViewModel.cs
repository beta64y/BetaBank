using BetaBank.Models;

namespace BetaBank.Areas.SuperAdmin.ViewModels
{
    public class BankAccountViewModel
    {
        public string Id { get; set; }
        public string AccountNumber { get; set; }
        public string IBAN { get; set; }
        public double Balance { get; set; }
        public BankAccountStatusModel AccountStatus { get; set; }
        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserProfilePhoto { get; set; }
    }
}
