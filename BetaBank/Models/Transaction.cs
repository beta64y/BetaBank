

namespace BetaBank.Models
{
    public class Transaction
    {
        public string Id { get; set; }
        public string ReceiptNumber { get; set; }


        public double Amount { get; set; }
        public double Commission { get; set; }
        public double BillingAmount { get; set; }
        public double CashbackAmount { get; set; }



        public DateTime TransactionDate { get; set; }

        public string PaidByTypeId { get; set; }

        public string PaidById { get; set; }


        public string DestinationTypeId { get; set; }

        public string DestinationId { get; set; }


        public string StatusId { get; set; }

        public string Icon {  get; set; }




    }

    
}

