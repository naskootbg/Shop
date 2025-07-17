using Backend.Data.Models;

namespace Backend.DTO
{
    public class SubscribersDTO
    {
        public List<Subscriber> ProductSubscriber { get; set; } = new List<Subscriber>();
        public List<Subscriber> TagSubscriber { get; set; } = new List<Subscriber>();
        public List<Subscriber> CategorySubscriber { get; set; } = new List<Subscriber>();
        public List<Subscriber> CategoryAndTafSubscriber { get; set; } = new List<Subscriber>();
    }
}
