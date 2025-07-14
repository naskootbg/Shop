using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class SitemapController : ControllerBase
{
    [HttpGet("generate")]
    public async Task<IActionResult> Generate()
    {
        var generator = new SitemapStaticGeneratorService(
            new HttpClient(),
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "sitemaps")
        );
        bool success = await generator.GenerateAllAsync();

        if (success)
            return Ok("Sitemap generated successfully.");
        else
            return StatusCode(500, "Sitemap generation failed.");
    }
}
