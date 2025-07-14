using Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

[ApiController]
[Route("api/[controller]")]
public class ImagePreloaderController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IWebHostEnvironment _env;
    private readonly AppDbContext _db;
    private readonly IServiceScopeFactory _scopeFactory;

    public ImagePreloaderController(IHttpClientFactory factory, IWebHostEnvironment env, AppDbContext db, IServiceScopeFactory scopeFactory )
    {
        _httpClient = factory.CreateClient();
        _env = env;
        _db = db;
        _scopeFactory = scopeFactory;
    }
    [HttpPost("reset")]
    public async Task<IActionResult> ResetImageDatabase()
    {
        var all = _db.ProcessedImages.ToList();
        _db.ProcessedImages.RemoveRange(all);
        await _db.SaveChangesAsync();

        return Ok(new { deleted = all.Count });
    }

    [HttpPost("preload")]
    public async Task<IActionResult> PreloadImages(CancellationToken cancellationToken)
    {
        var feedPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "gumisho.client", "public", "feeds.json");
        if (!System.IO.File.Exists(feedPath))
            return NotFound("feeds.json not found.");

        var json = await System.IO.File.ReadAllTextAsync(feedPath, cancellationToken);
        var feed = JsonSerializer.Deserialize<FeedModel>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var products = feed?.ProductsWrapper?.ProductList ?? new List<Product>();
        int downloaded = 0;

        // Use thread-safe increment for downloaded count
        var downloadedCounter = 0;

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = 200,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(products, parallelOptions, async (p, token) =>
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Check if already processed (inside the scope context)
            if (await db.ProcessedImages.AnyAsync(x => x.ProductCode == p.ProductCode, token))
                return;

            try
            {
                using var stream = await _httpClient.GetStreamAsync(p.ProductPic, token);
                using var image = await Image.LoadAsync(stream, token);

                // Resize max 600 preserving aspect ratio
                int maxSize = 600;
                if (image.Width > maxSize || image.Height > maxSize)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(maxSize, maxSize),
                        Mode = ResizeMode.Max
                    }));
                }

                // Watermark bottom-right
                var font = SystemFonts.CreateFont("Arial", image.Width / 20f);
                string text = "evtinOo.com";

                var textSize = TextMeasurer.MeasureSize(text, new TextOptions(font));
                var pos = new PointF(image.Width - textSize.Width - 10, image.Height - textSize.Height - 10);

                image.Mutate(x => x.DrawText(text, font, Color.White.WithAlpha(0.6f), pos));

                // Save image file
                var category = Slugify(p.Category);
                var name = Slugify(p.ProductName);
                if (name.Length > 60) name = name[..60];
                if (category.Length > 40) category = category[..40];
                var filename = $"{name}-{p.ProductCode}.webp";
                var folder = Path.Combine(_env.WebRootPath, "images", category);
                Directory.CreateDirectory(folder);
                var filePath = Path.Combine(folder, filename);

                await image.SaveAsync(filePath, new WebpEncoder { Quality = 75 }, token);

                // Save to DB
                db.ProcessedImages.Add(new ProcessedImage
                {
                    ProductCode = p.ProductCode,
                    SavedPath = $"/images/{category}/{filename}",
                    CreatedAt = DateTime.UtcNow
                });

                await db.SaveChangesAsync(token);

                Interlocked.Increment(ref downloadedCounter);
            }
            catch
            {
                // Optionally log errors here, but continue processing other images
            }
        });

        return Ok(new { downloaded = downloadedCounter });
    }


    private string Slugify(string input)
    {
        var str = input.ToLowerInvariant();
        str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", "-");
        str = System.Text.RegularExpressions.Regex.Replace(str, @"[^\w-]+", "");
        str = System.Text.RegularExpressions.Regex.Replace(str, @"-+", "-");
        return str.Trim('-');
    }

    private class FeedModel
    {
        [JsonPropertyName("products")]
        public ProductWrapper ProductsWrapper { get; set; }
    }

    private class ProductWrapper
    {
        [JsonPropertyName("product")]
        public List<Product> ProductList { get; set; }
    }

    private class Product
    {
        [JsonPropertyName("product_code")]
        public string ProductCode { get; set; }

        [JsonPropertyName("product_name")]
        public string ProductName { get; set; }

        [JsonPropertyName("product_pic")]
        public string ProductPic { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }
    }

    public class ProcessedImage
    {
        public int Id { get; set; }
        public string ProductCode { get; set; }
        public string SavedPath { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    [HttpPost("cleanup-illegal-files")]
    public IActionResult CleanupIllegalFiles()
    {
        var rootFolder = Path.Combine(_env.WebRootPath, "images");
        var invalidChars = Path.GetInvalidFileNameChars();

        int deletedCount = 0;
        var files = Directory.GetFiles(rootFolder, "*.*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);

            if (fileName.IndexOfAny(invalidChars) >= 0)
            {
                try
                {
                    System.IO.File.Delete(file);
                    deletedCount++;
                }
                catch (Exception ex)
                {
                    // Optionally log error
                    Console.WriteLine($"Failed to delete {file}: {ex.Message}");
                }
            }
        }

        return Ok(new { deleted = deletedCount });
    }

}
