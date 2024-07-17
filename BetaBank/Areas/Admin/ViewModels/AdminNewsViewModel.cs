using BetaBank.Areas.Admin.ViewModels;
using BetaBank.Models;

namespace BetaBank.Areas.Admin.ViewModels
{
    public class AdminNewsViewModel
    {
        public AdminSearchViewModel Search { get; set; }
        public List<News> News { get; set; }
    }
}
