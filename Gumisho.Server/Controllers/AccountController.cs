using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;



    public AccountController( UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
 
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var user = new IdentityUser { UserName = model.Username, Email = model.Email, PhoneNumber = model.PhoneNumber };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            return Ok(new { message = "User registered successfully" });
        }
        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, false);
        if (result.Succeeded)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            return Ok(user);
        }
        return Unauthorized();
    }


    [HttpGet("info")]
    public async Task<IActionResult> GetUserInfo()
    {
        var user = await _userManager.GetUserAsync(User);  // Get the currently logged-in user

        if (user == null)
        {
            return Ok(new { Message = "User not found" });
        }
        var role = await _userManager.GetRolesAsync(user);
        //var orders = await orderService.AllOrders(user.Id);
        //var addresses = await userService.ShowAddress(user.Id);
        var userInfo = new
        {
            user.UserName,
            user.Email,
            user.PhoneNumber,
            role,
            //orders,
            //addresses
        };

        return Ok(userInfo);
    }

     

    public class RegisterModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}