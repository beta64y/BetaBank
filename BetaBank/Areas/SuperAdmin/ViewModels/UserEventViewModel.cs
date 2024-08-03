using BetaBank.Models;
using System.ComponentModel.DataAnnotations;

namespace BetaBank.Areas.SuperAdmin.ViewModels
{
    public class UserEventViewModel
    {
        public string Section { get; set; }


        public string UserId { get; set; }
        public string UserUsername { get; set; }
        public string UserProfilePhoto { get; set; }
        public string Role { get; set; }
        
        public string Action { get; set; }


        public string EntityType { get; set; }
        public string EntityId { get; set; }

        public string Title { get; set; }

        public string EntityUserFirstName { get; set; }
        public string EntityUserLastName { get; set; }
        public string EntityUserProfilePhoto { get; set; }


        public DateTime Date { get; set; }
    }
}
