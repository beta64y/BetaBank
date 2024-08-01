using BetaBank.Models;

namespace BetaBank.Areas.SuperAdmin.ViewModels
{
    public class SupportViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Issue { get; set; }
        public DateTime CreatedDate { get; set; }
        public SupportStatusModel Status {  get; set; }
    }
}
