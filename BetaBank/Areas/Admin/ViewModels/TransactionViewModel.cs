using BetaBank.Models;
using System.Transactions;

namespace BetaBank.Areas.Admin.ViewModels
{
    public class TransactionViewModel
    {
        public string Id { get; set; }
        public string ReceiptNumber { get; set; }


        public double Amount { get; set; }
        public double Commission { get; set; }
        public double BillingAmount { get; set; }
        public double CashbackAmount { get; set; }



        public DateTime TransactionDate { get; set; }

        public TransactionTypeModel PaidByType { get; set; }

        public string PaidById { get; set; }
        public BankCardTypeModel PaidByCardType { get; set; }



        public TransactionTypeModel DestinationType { get; set; }

        public string DestinationId { get; set; }
        public BankCardTypeModel DestinationCardType { get; set; }


        public TransactionStatusModel Status { get; set; }
        public string Summary { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
    }
}
