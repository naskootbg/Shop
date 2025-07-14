using Backend.Data;
using Backend.Data.Models;
using Backend.DTO;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class FeedController : ControllerBase
    {
        private readonly AppDbContext db;
        

        public FeedController(AppDbContext _db)
        {
            db = _db;             
        }

   
        [HttpPost("all")]
        public async Task<IActionResult> AllHashes()
        {
            var feeds = await db.Feeds.AsNoTracking().ToListAsync();
            return Ok(feeds);   
        }
        [HttpPost("import")]
        public async Task<IActionResult> Import([FromBody] List<Feed> feeds)
        {
            await db.AddRangeAsync(feeds);
            await db.SaveChangesAsync();
            return Ok("added");
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddlHashes([FromBody] FeedDto dto)
        {
             
                var newFeed = new Feed()
                {
                    Name = dto.Name,
                    Hash = dto.Hash,
                };
                await db.Feeds.AddAsync(newFeed);
                await db.SaveChangesAsync();
                return Ok(newFeed);
            
        }
        [HttpPost("del")]
        public async Task<IActionResult> DelHashes([FromQuery] int id)
        {
            var hash = await db.Feeds.FindAsync(id);
            db.Remove(hash!);
            await db.SaveChangesAsync();
            return Ok("feed removed");
        }
    }
}
