using BetaBank.Areas.Support.ViewModels;
using BetaBank.Models;

namespace BetaBank.Areas.Moderator.ViewModels
{
    public class ModeratorSubscribersViewModel
    {
        public SupportSearchViewModel Search { get; set; }
        public List<Subscriber> Subscribers { get; set; }
    }
}
