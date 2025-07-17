using Backend.Contracts;
using Backend.Data;
using Backend.Data.Models;
using Backend.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services
{ 
    public class SubscriberService : ISubscriber
    {
        private readonly AppDbContext db;


        public SubscriberService(AppDbContext _db)
        {
            db = _db;
        }

        public async Task<List<Subscriber>> All()
        {
            return await db.Subscribers.AsNoTracking().ToListAsync();
        }

        public async Task<List<Subscriber>> CategorySub()
        {
            var subscribers = await db.Subscribers.AsNoTracking().Where(s => s.Category.Length > 0).ToListAsync();
        return subscribers;
        }

        public async Task<bool> IsSubscribed(string mail)
        {
            var subscriber = await db.Subscribers.AsNoTracking().Where(s => s.Email == mail).FirstOrDefaultAsync();
            return subscriber != null;
        }

        public async Task<SubscribersDTO> MySubs(string email)
        {
            var all = await db.Subscribers
                .AsNoTracking()
                .Where(s => s.Email == email)
                .ToListAsync();

            var dto = new SubscribersDTO
            {
                CategorySubscriber = all.Where(s => !string.IsNullOrEmpty(s.Category) && string.IsNullOrEmpty(s.Tag)).ToList(),
                ProductSubscriber = all.Where(s => !string.IsNullOrEmpty(s.ProductCode)).ToList(),
                TagSubscriber = all.Where(s => !string.IsNullOrEmpty(s.Tag) && string.IsNullOrEmpty(s.Category)).ToList(),
                CategoryAndTafSubscriber = all.Where(s => !string.IsNullOrEmpty(s.Tag) && !string.IsNullOrEmpty(s.Category)).ToList(),
            };

            return dto;
        }


        public async Task<List<Subscriber>> ProductSub()
        {
            var subscribers = await db.Subscribers.AsNoTracking().Where(s => s.ProductCode.Length > 0).ToListAsync();       
            return subscribers;
        }

        public async Task<Subscriber> SubscribeMe(Subscriber data)
        {
             
            await db.Subscribers.AddAsync(data);
            await db.SaveChangesAsync();
            return data;
        }

        public async Task<List<Subscriber>> TagSub()
        {
            var subscribers = await db.Subscribers.AsNoTracking().Where(s => s.Tag.Length > 0).ToListAsync();
            return subscribers;
        }
    }
}
