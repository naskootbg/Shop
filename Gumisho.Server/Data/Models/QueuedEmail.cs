namespace Backend.Data.Models
{
    public class QueuedEmail
    {
        public int Id { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public bool IsSent { get; set; }
        public DateTime? SentAt { get; set; }
        public bool Opened { get; set; }
        public bool Clicked { get; set; }
        public string? CampaignId { get; set; }
        public Guid TrackingToken { get; set; } = Guid.NewGuid(); // ✅ NEW
        public List<EmailAttachment> Attachments { get; set; } = new();
    }
}