using BetaBank.Utils.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace BetaBank.Models
{
    public class Transaction
    {
        public int Id { get; set; }


        public int SourceCardId { get; set; }
        public  BankCard SourceCard { get; set; }

        
        public int DestinationCardId { get; set; }
        public virtual BankCard DestinationCard { get; set; }

        public decimal Amount { get; set; }
        public decimal Commission { get; set; }

        public DateTime TransactionTime { get; set; }
        public string Description { get; set; }

    }
}
