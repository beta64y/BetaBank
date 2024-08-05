using BetaBank.Utils.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BetaBank.Areas.SuperAdmin.ViewComponents
{
    public class EventFilterViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var actions = Enum.GetValues(typeof(UserActionType)).Cast<UserActionType>().ToList();
            var sections = Enum.GetValues(typeof(SectionType)).Cast<SectionType>().ToList();
            var entities = Enum.GetValues(typeof(EntityType)).Cast<EntityType>().ToList();

            ViewData["Actions"] = actions;
            ViewData["Sections"] = sections;
            ViewData["Entities"] = entities;

            return View();
        }
    }
}
