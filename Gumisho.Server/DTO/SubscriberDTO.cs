namespace Backend.DTO
{
    public class SubscriberDTO
    {
        public int Id { get; set; }
        public int? Push { get; set; } = 0;
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Tag { get; set; }
        public string? Category { get; set; }
        public string? ProductCode { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinPercent { get; set; } = 0;
    }
}
