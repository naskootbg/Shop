using Backend.Contracts;
using Backend.Data;
using Backend.Data.Models;
using Backend.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscribeController : ControllerBase
    {
        private readonly ISubscriber service;


        public SubscribeController(ISubscriber _service)
        {
             service = _service;
        }

        [HttpPost("all")]
        public async Task<IActionResult> All()
        {
            var all = await service.All();
            return Ok(all);
        }
        [HttpPost("cat-all")]
        public async Task<IActionResult> CatAll()
        {
            var all = await service.CategorySub();
            return Ok(all);
        }
        [HttpPost("tag-all")]
        public async Task<IActionResult> TagAll()
        {
            var all = await service.TagSub();
            return Ok(all);
        }
        [HttpPost("product-all")]
        public async Task<IActionResult> PAll()
        {
            var all = await service.ProductSub();
            return Ok(all);
        }
        [HttpPost("check")]
        public async Task<IActionResult> Check([FromQuery] string mail)
        {
             
            return Ok(await service.IsSubscribed(mail));
        }

        [HttpPost("sub")]
        public async Task<IActionResult> JoinList([FromBody] SubscriberDTO dto)
        {
            var user = new Subscriber();
            if (dto.Email.IsNullOrEmpty())
            {
                user.Email = UserEmail();
                user.UserName = UserName();
            }
            else
            {
                user.Email = dto.Email;
                user.UserName = dto.UserName;
            }
 
                user.ProductCode = dto.ProductCode;
                user.MaxPrice = dto.MaxPrice;
                user.MinPercent = dto.MinPercent;
                user.Push = dto.Push;                            
                user.Category = dto.Category;            
                user.Tag = dto.Tag;
           var data = await service.SubscribeMe(user);
            return Ok(data);
        }

        [HttpPost("my")]
        public async Task<IActionResult> MySubs([FromQuery] string email)
        {
            var all = await service.MySubs(email);
            return Ok(all);
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
