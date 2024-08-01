using BetaBank.Models;

namespace BetaBank.Areas.SuperAdmin.ViewModels
{
    public class UserCashBackViewModel
    {
        public string Id { get; set; }

        public string CashBackNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public double Balance { get; set; }
    }
}
