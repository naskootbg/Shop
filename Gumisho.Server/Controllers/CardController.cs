using Backend.Data;
using Backend.Data.Models;
using Backend.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly AppDbContext db;


        public CardController(AppDbContext _db)
        {
            db = _db;
        }
        [HttpPost("all")]
        public async Task<IActionResult> Allcards()
        {
            var cards = await db.Cards.AsNoTracking().ToListAsync();
            return Ok(cards);
        }
        [HttpPost("del-item")]
        public async Task<IActionResult> ClearCard([FromBody] CardDTO card)
        {
            var curr = await db.Cards.FindAsync(card.Id);
            if (curr == null)
            {
                return NotFound("Card not found");
            }

            if (curr.ProductCodes == null || !curr.ProductCodes.Remove(card.Code))
            {
                return BadRequest("Product code not found in the card");
            }

            await db.SaveChangesAsync();
            return Ok($"{card.Code} removed");
        }


        [HttpPost("clear")]
        public async Task<IActionResult> DelItemCard([FromQuery] int id)
        {
            var card = await db.Cards.FindAsync(id);
            db.Cards.Remove(card);
            await db.SaveChangesAsync();
            return Ok($"{id} removed");
        }
        [HttpPost("show")]
        public async Task<IActionResult> ShiwCard([FromQuery] int id)
        {
            var card = await db.Cards.FindAsync(id);
            
            return Ok(card);
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CardDTO carddto)
        {
            if (carddto.Username.IsNullOrEmpty() || carddto.Email.IsNullOrEmpty())
            {
                var checkCard = await db.Cards.AsNoTracking().Where(c => c.Email == UserEmail()).FirstOrDefaultAsync();
                if (checkCard == null) {
                    var card = new Card()
                    {
                        Email = UserEmail(),
                        Username = UserName(),
                    };
                    await db.Cards.AddAsync(card);
                    await db.SaveChangesAsync();
                    return Ok(card);
                }
                else
                {
                    return Ok(checkCard);
                }
                    
            }
            else {
                var checkCard = await db.Cards.AsNoTracking().Where(c => c.Email == carddto.Email).FirstOrDefaultAsync();
                if (checkCard == null)
                {
                    var card = new Card()
                    {
                        Email = carddto.Email,
                        Username = carddto.Username,
                    };
                    await db.Cards.AddAsync(card);
                    await db.SaveChangesAsync();
                    return Ok(card);
                }
                else
                {
                    return Ok(checkCard);
                }
            }
           
        }
        [HttpPost("add-item")]
        public async Task<IActionResult> AddItem([FromBody] CardDTO card)
        {
            var curr = await db.Cards.FindAsync(card.Id);
            if (curr == null)
            {
                return NotFound("Card not found");
            }

            if (string.IsNullOrWhiteSpace(card.Code))
            {
                return BadRequest("Product code cannot be empty");
            }

            if (curr.ProductCodes == null)
            {
                curr.ProductCodes = new List<string>();
            }

            if (curr.ProductCodes.Contains(card.Code))
            {
                return BadRequest("Product code already exists in the card");
            }

            curr.ProductCodes.Add(card.Code);
            await db.SaveChangesAsync();

            return Ok(card);
        }

        private string UserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
        private string UserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        }

        private string UserName()
        {
            return User.Identity?.Name ?? string.Empty;
        }
    }
}
