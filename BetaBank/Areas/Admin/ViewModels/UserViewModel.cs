namespace BetaBank.Areas.Admin.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Banned { get; set; }
        public string ProfilePhoto {  get; set; }
        public string Email { get; set; }
        public int Age {get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
