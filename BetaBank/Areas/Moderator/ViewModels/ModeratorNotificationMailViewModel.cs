using BetaBank.Areas.Support.ViewModels;
using BetaBank.Models;

namespace BetaBank.Areas.Moderator.ViewModels
{
    public class ModeratorNotificationMailViewModel
    {
        public List<SendedNotificationMail> NotificationMails { get; set; }
        public SupportSearchViewModel Search { get; set; }

    }
}
