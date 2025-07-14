using Microsoft.AspNetCore.Identity;

namespace Backend.Data.Models
{
    public class Card
    {
        public int Id { get; set; }
        public List<string>? ProductCodes { get; set; }
        public string? Email { get; set; }
        public IdentityUser? User { get; set; }
        public string? Username { get; set; }

    }
}
