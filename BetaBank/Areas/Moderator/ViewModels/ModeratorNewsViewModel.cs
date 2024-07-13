using BetaBank.Areas.Support.ViewModels;
using BetaBank.Models;

namespace BetaBank.Areas.Moderator.ViewModels
{
    public class ModeratorNewsViewModel
    {
        public SupportSearchViewModel Search { get; set; }
        public List<News> News { get; set; }
    }
}
