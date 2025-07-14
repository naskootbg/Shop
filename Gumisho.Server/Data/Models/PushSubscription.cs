using Microsoft.AspNetCore.Identity;

namespace Backend.Data.Models
{
    public class PushSubscription
    {
        public int Id { get; set; }
        public string Endpoint { get; set; }
        public string P256dh { get; set; }
        public string Auth { get; set; }

        // Associate with a User

        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }
    
        public int? OrderId { get; set; }
        
    }
}
