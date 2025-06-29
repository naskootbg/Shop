using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfitshareController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string outputPath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "..", "gumisho.client", "public", "feeds.json")
        );



        public ProfitshareController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("merge")]
      
        public async Task<IActionResult> MergeFeeds([FromBody] List<string> hashes)
        {
            if (hashes == null || !hashes.Any())
                return BadRequest("Use: /api/profitshare/merge?hashes=abc&hashes=def");

            var allProducts = new List<XElement>();

            foreach (var hash in hashes)
            {
                var url = $"https://profitshare.bg/affiliate-feed/export/?hash={hash}";

                try
                {
                    var client = _httpClientFactory.CreateClient();
                    var response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        continue;
                    }

                    var stream = await response.Content.ReadAsStreamAsync();
                    var doc = XDocument.Load(stream);

                    var products = doc.Descendants("product");
                    allProducts.AddRange(products);
                }
                catch (Exception ex)
                {
                    // Optional: log or collect errors
                }
            }

            // Wrap merged <product> elements in <products> root for JSON conversion
            var mergedDoc = new XDocument(
                new XElement("products", allProducts)
            );

            var json = JsonConvert.SerializeXNode(mergedDoc, Formatting.Indented, omitRootObject: false);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            await System.IO.File.WriteAllTextAsync(outputPath, json);

            return Ok("done");
        }
    }
}

