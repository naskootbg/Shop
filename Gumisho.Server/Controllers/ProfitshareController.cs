using Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfitshareController : ControllerBase
    {
        private readonly AppDbContext db;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string outputPath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "..", "gumisho.client", "public", "feeds.json")
        );



        public ProfitshareController(IHttpClientFactory httpClientFactory, AppDbContext _db)
        {
            _httpClientFactory = httpClientFactory;
            db = _db;
        }

        [HttpPost("merge")]
        public async Task<IActionResult> MergeFeeds()
        {
            var hashes = await db.Feeds.AsNoTracking().ToListAsync();
            if (hashes == null || !hashes.Any())
                return BadRequest("Use: /api/profitshare/merge?hashes=abc&hashes=def");

            var allProducts = new List<XElement>();

            foreach (var hash in hashes)
            {
                var url = $"https://profitshare.bg/affiliate-feed/export/?hash={hash.Hash}";

                try
                {
                    var client = _httpClientFactory.CreateClient();
                    var response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                        continue;

                    var stream = await response.Content.ReadAsStreamAsync();
                    var doc = XDocument.Load(stream);

                    var products = doc.Descendants("product");
                    allProducts.AddRange(products);
                }
                catch (Exception ex)
                {
                    // Optional: log or ignore
                }
            }

            // ✅ Filter out products without price_discounted or price_discounted == 0
            allProducts = allProducts
                .Where(p =>
                {
                    var price = (string?)p.Element("price_discounted");
                    return !string.IsNullOrWhiteSpace(price) &&
                           decimal.TryParse(price, out var val) &&
                           val > 0;
                })
                .ToList();

            // ✅ Wrap for JSON
            var mergedDoc = new XDocument(new XElement("products", allProducts));
            var json = JsonConvert.SerializeXNode(mergedDoc, Formatting.Indented, omitRootObject: false);

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            await System.IO.File.WriteAllTextAsync(outputPath, json);

            return Ok("done");
        }

    }
}

