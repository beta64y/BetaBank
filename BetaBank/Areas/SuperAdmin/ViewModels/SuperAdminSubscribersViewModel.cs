using BetaBank.Areas.SuperAdmin.ViewModels;
using BetaBank.Models;

namespace BetaBank.Areas.SuperAdmin.ViewModels
{
    public class SuperAdminSubscribersViewModel
    {
        public SuperAdminSearchViewModel Search { get; set; }
        public List<Subscriber> Subscribers { get; set; }
    }
}
