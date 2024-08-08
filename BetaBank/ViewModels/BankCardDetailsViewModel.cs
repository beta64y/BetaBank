using BetaBank.Models;

namespace BetaBank.ViewModels
{
    public class BankCardDetailsViewModel
    {
        public string Id { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public DateTime ExpiryDate { get; set; }
        public double Balance { get; set; }
        public BankCardStatusModel CardStatus { get; set; }
        public BankCardTypeModel CardType { get; set; }
        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserProfilePhoto { get; set; }
    }
}
