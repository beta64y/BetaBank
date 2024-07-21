using BetaBank.Utils.Enums;
using BetaBank.Models;
using System.ComponentModel.DataAnnotations;

namespace BetaBank.Areas.Admin.ViewModels
{
    public class UserBankCardViewModel
    {
        public string Id { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public DateTime ExpiryDate { get; set; }
        public double Balance { get; set; }
        public BankCardStatusModel CardStatus { get; set;}
        public BankCardTypeModel CardType { get; set;}
    }
}
