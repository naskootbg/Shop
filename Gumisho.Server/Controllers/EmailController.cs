using Backend.Data;
using Backend.Data.Models;
using Backend.DTO;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly EmailService service;

        public EmailController(AppDbContext db, EmailService _service)
        {
            _db = db;
            service = _service;
        }
        [HttpGet("track-open")]
        public IActionResult TrackOpen([FromQuery] Guid token)
        {
            var email = _db.QueuedEmails.FirstOrDefault(e => e.TrackingToken == token);
            if (email != null && !email.Opened)
            {
                email.Opened = true;
                _db.SaveChanges();
            }

            return File(new byte[1], "image/gif");
        }
        [HttpGet("track-click")]
        public IActionResult TrackClick([FromQuery] Guid token, [FromQuery] string target)
        {
            var email = _db.QueuedEmails.FirstOrDefault(e => e.TrackingToken == token);
            if (email != null && !email.Clicked)
            {
                email.Clicked = true;
                _db.SaveChanges();
            }

            return Redirect(target);
        }

        [HttpGet("get")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Getit()
        {
            var email = await service.GetSmtpSettingsAsync();

            return Ok(email);
        }
        [HttpPost("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSet(SMTPDTO dto)
        {
            await service.UpdateSmtpSettingsAsync(dto);

            return Ok("success");
        }
        [HttpPost("send")]
        public async Task<IActionResult> Send(QueuedEmail dto)
        {
            await service.SendEmailAsync(dto);

            return Ok("success");
        }
    }
}
