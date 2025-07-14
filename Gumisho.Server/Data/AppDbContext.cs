using Backend.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static ImagePreloaderController;

namespace Backend.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
           public DbSet<Feed> Feeds { get; set; }
        public DbSet<ProcessedImage> ProcessedImages { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<EmailAttachment> EmailAttachments { get; set; }
        public DbSet<Mail> Mails { get; set; }
        public DbSet<PushSubscription> PushSubscriptions { get; set; }
        public DbSet<QueuedEmail> QueuedEmails { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            //Database.Migrate();
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity<ServiceOrder>().Property(x => x.QuantityS).HasPrecision(11, 6);

        }
    }
}