using BetaBank.Areas.SuperAdmin.ViewModels;
using BetaBank.Models;

namespace BetaBank.Areas.SuperAdmin.ViewModels
{
    public class SuperAdminNotificationMailViewModel
    {
        public List<SendedNotificationMail> NotificationMails { get; set; }
        public SuperAdminSearchViewModel Search { get; set; }

    }
}
