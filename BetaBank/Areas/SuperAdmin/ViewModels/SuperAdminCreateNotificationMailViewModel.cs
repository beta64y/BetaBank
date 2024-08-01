using System.ComponentModel.DataAnnotations;

namespace BetaBank.Areas.SuperAdmin.ViewModels
{
    public class SuperAdminCreateNotificationMailViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
