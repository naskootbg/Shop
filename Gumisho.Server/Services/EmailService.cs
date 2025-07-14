namespace Backend.Services
{
    using Backend.Data;
    using Backend.Data.Models;
    using Backend.DTO;
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using Microsoft.EntityFrameworkCore;
    using MimeKit;
    public class EmailService
    {
        private readonly AppDbContext _context;
        public EmailService(AppDbContext context)
        {
            _context = context;
        }
        public async Task UpdateSmtpSettingsAsync(SMTPDTO dto)
        {
            var settings = await _context.Mails.AsNoTracking().FirstOrDefaultAsync();
            if (settings == null)
            {
                var newSettings = new Mail
                {
                    SmtpServer = dto.SmtpServer,
                    SmtpPort = dto.SmtpPort,
                    SmtpUser = dto.SmtpUser,
                    SmtpPass = dto.SmtpPass
                };
                newSettings.SmtpServer = dto.SmtpServer;
                newSettings.SmtpPort = dto.SmtpPort;
                newSettings.SmtpUser = dto.SmtpUser;
                newSettings.SmtpPass = dto.SmtpPass;
                await _context.Mails.AddAsync(newSettings);
                await _context.SaveChangesAsync();
            }
            else
            {
                settings.SmtpServer = dto.SmtpServer;
                settings.SmtpPort = dto.SmtpPort;
                settings.SmtpUser = dto.SmtpUser;
                settings.SmtpPass = dto.SmtpPass;
                _context.Mails.Update(settings);
                await _context.SaveChangesAsync();
            }



        }
        public async Task<SMTPDTO> GetSmtpSettingsAsync()
        {
            var settings = await _context.Mails.AsNoTracking().FirstOrDefaultAsync();
            if (settings == null)
            {
                throw new InvalidOperationException("SMTP settings not found.");
            }
            return new SMTPDTO
            {
                SmtpServer = settings.SmtpServer,
                SmtpPort = settings.SmtpPort,
                SmtpUser = settings.SmtpUser,
                SmtpPass = settings.SmtpPass
            };
        }
        public async Task SendEmailAsync(QueuedEmail email)
        {
            var spmtpSettings = await GetSmtpSettingsAsync();
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(spmtpSettings.SmtpUser));
            message.To.Add(MailboxAddress.Parse(email.ToEmail));
            message.Subject = email.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = email.HtmlBody
            };

            foreach (var attachment in email.Attachments)
            {
                builder.Attachments.Add(attachment.FileName, attachment.Content);
            }

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();

            await client.ConnectAsync(
                spmtpSettings.SmtpServer,
                spmtpSettings.SmtpPort,
                SecureSocketOptions.SslOnConnect // <-- switch this if using port 465
            );

            await client.AuthenticateAsync(spmtpSettings.SmtpUser, spmtpSettings.SmtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
