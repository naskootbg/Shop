namespace Backend.Services
{
    using Backend.Data;
    using Hangfire;
    using Microsoft.EntityFrameworkCore;
    public class BulkEmailJob
    {
        private readonly AppDbContext _db;
        private readonly EmailService _emailService;

        public BulkEmailJob(AppDbContext db, EmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task ProcessDailyBatch()
        {
            var unsent = await _db.QueuedEmails
                .Include(e => e.Attachments)
                .Where(e => !e.IsSent)
                .OrderBy(e => e.Id)
                .Take(200)
                .ToListAsync();

            foreach (var email in unsent)
            {
                try
                {
                    await _emailService.SendEmailAsync(email);
                    email.IsSent = true;
                    email.SentAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    // Optional: log error
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}
