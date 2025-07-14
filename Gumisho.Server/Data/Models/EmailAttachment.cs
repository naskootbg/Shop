namespace Backend.Data.Models
{
    public class EmailAttachment
    {
        public int Id { get; set; }
        public int QueuedEmailId { get; set; }

        public string FileName { get; set; } = null!;
        public byte[] Content { get; set; } = null!;
        public QueuedEmail? QueuedEmail { get; set; } = null!;
    }
}
