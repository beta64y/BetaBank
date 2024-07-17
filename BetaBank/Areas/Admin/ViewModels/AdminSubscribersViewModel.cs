using BetaBank.Areas.Admin.ViewModels;
using BetaBank.Models;

namespace BetaBank.Areas.Admin.ViewModels
{
    public class AdminSubscribersViewModel
    {
        public AdminSearchViewModel Search { get; set; }
        public List<Subscriber> Subscribers { get; set; }
    }
}
