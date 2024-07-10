namespace BetaBank.Models
{
    public class SupportStatus
    {
        public string Id { get; set; }
        public string SupportId { get; set; }
        public Support Support { get; set; }
        public string StatusId { get; set; }
        public SupportStatusModel Status { get; set; }
    }
}
