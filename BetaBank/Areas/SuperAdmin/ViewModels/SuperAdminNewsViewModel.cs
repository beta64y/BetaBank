using BetaBank.Areas.Admin.ViewModels;
using BetaBank.Models;

namespace BetaBank.Areas.SuperAdmin.ViewModels
{
    public class SuperAdminNewsViewModel
    {
        public AdminSearchViewModel Search { get; set; }
        public List<News> News { get; set; }
    }
}
