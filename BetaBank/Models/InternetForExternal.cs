namespace BetaBank.Models
{
    public class InternetForExternal
    {
        public int Id { get; set; }
        public string SubscriberCode { get; set; }
        public string AppointmentType { get; set; } //Individuals    // Commercial consumers
        public string Title { get; set; }
    }
}
