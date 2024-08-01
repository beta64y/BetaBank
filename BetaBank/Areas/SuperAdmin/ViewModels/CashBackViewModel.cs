namespace BetaBank.Areas.SuperAdmin.ViewModels
{
    public class CashBackViewModel
    {
        public string Id { get; set; }

        public string CashBackNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public double Balance { get; set; }
        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserProfilePhoto { get; set; }
    }
}
