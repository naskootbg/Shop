using Backend.Data.Models;
using Backend.DTO;

namespace Backend.Contracts
{
    public interface ISubscriber
    {
        public Task<bool> IsSubscribed(string mail);
        public Task<Subscriber> SubscribeMe(Subscriber data);
        public Task<List<Subscriber>> All();
        public Task<List<Subscriber>> CategorySub();
        public Task<List<Subscriber>> TagSub();
        public Task<List<Subscriber>> ProductSub();
        public Task<SubscribersDTO> MySubs(string email);
    }
}
