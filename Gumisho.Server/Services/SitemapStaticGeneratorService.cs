using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class SitemapStaticGeneratorService
{
    private readonly HttpClient _httpClient;
    private readonly string _outputFolder;

    private const string FeedUrl = "https://evtinoo.com/feeds.json";

    public SitemapStaticGeneratorService(HttpClient httpClient, string outputFolder)
    {
        _httpClient = httpClient;
        _outputFolder = outputFolder;
    }

    public async Task<bool> GenerateAllAsync()
    {
        try
        {
            Console.WriteLine($"🧾 Generating sitemap files into: {_outputFolder}");
            Directory.CreateDirectory(_outputFolder);

            var json = await _httpClient.GetStringAsync(FeedUrl);
            var feed = JsonSerializer.Deserialize<FeedModel>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var products = feed?.ProductsWrapper?.ProductList ?? new List<Product>();

            // Filter discounted products
            var discountedProducts = products
                .Where(p => p.PriceDiscounted > 0 && p.PriceDiscounted < p.PriceVat)
                .ToList();

            // Categories
            var categories = products
                .Select(p => p.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            // Generate each sitemap part
            await File.WriteAllTextAsync(Path.Combine(_outputFolder, "sitemap-categories.xml"),
                GenerateCategoriesXml(categories));

            await File.WriteAllTextAsync(Path.Combine(_outputFolder, "sitemap-discounts.xml"),
                GenerateDiscountsXml());

            int half = (int)Math.Ceiling(discountedProducts.Count / 2.0);

            await File.WriteAllTextAsync(Path.Combine(_outputFolder, "sitemap-products1.xml"),
                GenerateProductsXml(discountedProducts.Take(half).ToList()));

            await File.WriteAllTextAsync(Path.Combine(_outputFolder, "sitemap-products2.xml"),
                GenerateProductsXml(discountedProducts.Skip(half).ToList()));

            // Generate sitemap index
            await File.WriteAllTextAsync(Path.Combine(_outputFolder, "sitemap.xml"),
                GenerateSitemapIndexXml());

            Console.WriteLine("✅ All sitemap files generated successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Sitemap generation failed: {ex}");
            return false;
        }
    }

    private string GenerateSitemapIndexXml()
    {
        var baseUrl = "https://evtinoo.com/sitemaps";

        var files = new[]
        {
            "sitemap-categories.xml",
            "sitemap-discounts.xml",
            "sitemap-products1.xml",
            "sitemap-products2.xml"
        };

        var sb = new StringBuilder();
        sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
        sb.AppendLine(@"<sitemapindex xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">");

        foreach (var file in files)
        {
            sb.AppendLine("  <sitemap>");
            sb.AppendLine($"    <loc>{baseUrl}/{file}</loc>");
            sb.AppendLine($"    <lastmod>{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}</lastmod>");
            sb.AppendLine("  </sitemap>");
        }

        sb.AppendLine("</sitemapindex>");
        return sb.ToString();
    }

    private string GenerateCategoriesXml(List<string> categories)
    {
        var sb = new StringBuilder();
        sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
        sb.AppendLine(@"<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">");

        foreach (var cat in categories)
        {
            var slug = Slugify(cat);
            sb.AppendLine("<url>");
            sb.AppendLine($"  <loc>https://evtinoo.com/category/{slug}</loc>");
            sb.AppendLine("  <changefreq>weekly</changefreq>");
            sb.AppendLine($"  <lastmod>{DateTime.UtcNow:yyyy-MM-dd}</lastmod>");
            sb.AppendLine("</url>");
        }

        sb.AppendLine("</urlset>");
        return sb.ToString();
    }

    private string GenerateDiscountsXml()
    {
        var sb = new StringBuilder();
        sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
        sb.AppendLine(@"<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">");

        for (int discount = 5; discount <= 70; discount += 5)
        {
            sb.AppendLine("<url>");
            sb.AppendLine($"  <loc>https://evtinoo.com/discount/{discount}</loc>");
            sb.AppendLine("  <changefreq>daily</changefreq>");
            sb.AppendLine($"  <lastmod>{DateTime.UtcNow:yyyy-MM-dd}</lastmod>");
            sb.AppendLine("</url>");
        }

        sb.AppendLine("</urlset>");
        return sb.ToString();
    }

    private string GenerateProductsXml(List<Product> products)
    {
        var sb = new StringBuilder();
        sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
        sb.AppendLine(@"<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"" 
                       xmlns:image=""http://www.google.com/schemas/sitemap-image/1.1"">");

        foreach (var p in products)
        {
            var encoded = Uri.EscapeDataString(p.ProductCode);
            var slug = Slugify(p.ProductName);
            var url = $"https://evtinoo.com/{encoded}/{slug}";

            // Generate local image path
            var category = Slugify(p.Category ?? "");
            var name = Slugify(p.ProductName ?? "");
            if (name.Length > 60) name = name.Substring(0, 60);
            if (category.Length > 40) category = category.Substring(0, 40);

            var relativePath = $"/images/{category}/{name}-{p.ProductCode}.webp";
            var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath.TrimStart('/'));

            sb.AppendLine("<url>");
            sb.AppendLine($"  <loc>{url}</loc>");
            sb.AppendLine("  <changefreq>weekly</changefreq>");
            sb.AppendLine($"  <lastmod>{DateTime.UtcNow:yyyy-MM-dd}</lastmod>");
            if (System.IO.File.Exists(physicalPath))
            {
                var imageUrl = $"https://evtinoo.com{relativePath}";
                sb.AppendLine("  <image:image>");
                sb.AppendLine($"    <image:loc>{EscapeXml(imageUrl)}</image:loc>");
                sb.AppendLine($"    <image:title>{EscapeXml(p.ProductName)}</image:title>");
                sb.AppendLine("  </image:image>");
            }

            sb.AppendLine("</url>");
        }

        sb.AppendLine("</urlset>");
        return sb.ToString();
    }

    private static string Slugify(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "";

        string str = input.ToLowerInvariant();
        str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", "-");
        str = System.Text.RegularExpressions.Regex.Replace(str, @"[^\w\-а-яА-Я]", "");
        return str;
    }

    private static string EscapeXml(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";

        return System.Security.SecurityElement.Escape(input);
    }

    // DTO classes
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

        [JsonPropertyName("price_vat")]
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal PriceVat { get; set; }

        [JsonPropertyName("price_discounted")]
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal PriceDiscounted { get; set; }
    }

    private class DecimalStringConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions opts)
        {
            if (reader.TokenType == JsonTokenType.String &&
                decimal.TryParse(reader.GetString(), out var v))
                return v;
            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetDecimal();
            return 0m;
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions opts)
            => writer.WriteNumberValue(value);
    }
}
