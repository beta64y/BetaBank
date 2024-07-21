using BetaBank.Models;
using System.Transactions;

namespace BetaBank.ViewModels
{
    public class TransactionViewModel
    {
        public TransactionTypeModel PaidByType { get; set; }

        public string PaidById { get; set; }


        public TransactionTypeModel DestinationType { get; set; }

        public string DestinationId { get; set; }

        public double Amount { get; set; }
        public double Commission { get; set; }
    }
}
