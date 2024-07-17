using BetaBank.Areas.Admin.ViewModels;
using BetaBank.Models;

namespace BetaBank.Areas.Admin.ViewModels
{
    public class AdminNotificationMailViewModel
    {
        public List<SendedNotificationMail> NotificationMails { get; set; }
        public AdminSearchViewModel Search { get; set; }

    }
}
