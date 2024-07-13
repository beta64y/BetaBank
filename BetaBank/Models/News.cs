namespace BetaBank.Models
{
    public class News
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FirstImage { get; set; }
        public string SecondImage { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
