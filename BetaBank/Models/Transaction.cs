using BetaBank.Utils.Enums;
using Humanizer;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.Xml;
using System.Security.Principal;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace BetaBank.Models
{
    public class Transaction
    {
        public string Id { get; set; }
        public string ReceiptNumber { get; set; }


        public string SourceCardNumber { get; set; }
        public BankCard SourceCard { get; set; }



        public decimal Amount { get; set; }
        public decimal Commission { get; set; }
        public decimal BillingAmount { get; set; }
        public decimal CashbackAmount { get; set; }



        public DateTime TransactionDate { get; set; }

        public string PaidByTypeId { get; set; }
        public TransactionTypeModel PaidByType { get; set; }

        public string PaidById { get; set; }


        public string DestinationTypeId { get; set; }
        public TransactionTypeModel DestinationType { get; set; }

        public string DestinationId { get; set; }


        public string StatusId { get; set; }
        public TransactionStatusModel Status { get; set; }




    }

    
}

