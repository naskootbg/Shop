using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class SitemapGeneratorService
{
    private readonly HttpClient _httpClient;
    private readonly string _feedUrl = "https://yourdomain.com/feeds.json"; // Update with your actual URL
    private readonly string _outputPath = Path.GetFullPath(
        Path.Combine(Directory.GetCurrentDirectory(), "..", "gumisho.client", "public", "sitemap.xml")
    );

    public SitemapGeneratorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task GenerateSitemapAsync()
    {
        // Fetch JSON feed
        var jsonString = await _httpClient.GetStringAsync(_feedUrl);

        // Deserialize your feed model (define your own model classes)
        var feed = JsonSerializer.Deserialize<FeedModel>(jsonString);

        var products = feed?.Products?
            .Where(p => p.PriceDiscounted > 0 && p.PriceDiscounted < p.PriceVat)
            .ToList() ?? new List<Product>();

        var categories = products.Select(p => p.Category).Distinct();

        var xml = new StringBuilder();
        xml.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
        xml.AppendLine(@"<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"" 
                          xmlns:image=""http://www.google.com/schemas/sitemap-image/1.1"">");

        // Add discount pages (1% to 70%)
        for (int discount = 1; discount <= 70; discount++)
        {
            xml.AppendLine("<url>");
            xml.AppendLine($"  <loc>https://gumisho.bg/discount/{discount}</loc>");
            xml.AppendLine("  <changefreq>daily</changefreq>");
            xml.AppendLine("</url>");
        }

        // Add category URLs with slugified categories
        foreach (var cat in categories)
        {
            var slug = Slugify(cat);
            xml.AppendLine("<url>");
            xml.AppendLine($"  <loc>https://gumisho.bg/category/{slug}</loc>");
            xml.AppendLine("  <changefreq>weekly</changefreq>");
            xml.AppendLine("</url>");
        }

        // Add products with images
        foreach (var p in products)
        {
            var slug = Slugify(p.ProductName);
            var url = $"https://gumisho.bg/{p.ProductCode}/{slug}";

            xml.AppendLine("<url>");
            xml.AppendLine($"  <loc>{url}</loc>");
            xml.AppendLine("  <changefreq>weekly</changefreq>");
            if (!string.IsNullOrEmpty(p.ProductPic))
            {
                xml.AppendLine("  <image:image>");
                xml.AppendLine($"    <image:loc>{p.ProductPic}</image:loc>");
                xml.AppendLine($"    <image:title>{EscapeXml(p.ProductName)}</image:title>");
                xml.AppendLine("  </image:image>");
            }
            xml.AppendLine("</url>");
        }

        xml.AppendLine("</urlset>");

        // Save sitemap.xml
        Directory.CreateDirectory(Path.GetDirectoryName(_outputPath)!);
        await File.WriteAllTextAsync(_outputPath, xml.ToString(), Encoding.UTF8);

        // Ping search engines
        await PingSearchEnginesAsync();
    }

    private async Task PingSearchEnginesAsync()
    {
        var sitemapUrl = "https://gumisho.bg/sitemap.xml";

        var searchEnginePingUrls = new[]
        {
            $"https://www.google.com/ping?sitemap={Uri.EscapeDataString(sitemapUrl)}",
            $"https://www.bing.com/ping?sitemap={Uri.EscapeDataString(sitemapUrl)}"
        };

        foreach (var pingUrl in searchEnginePingUrls)
        {
            try
            {
                var response = await _httpClient.GetAsync(pingUrl);
                if (!response.IsSuccessStatusCode)
                {
                    // Optional: log failed ping
                }
            }
            catch (Exception ex)
            {
                // Optional: log exception
            }
        }
    }

    // Helper slugify function: replace spaces with dash, lowercase, remove invalid chars (implement as you like)
    private string Slugify(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "";

        // For example, use System.Text.RegularExpressions here or your existing slugify logic
        string str = input.ToLowerInvariant();
        str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", "-");
        str = System.Text.RegularExpressions.Regex.Replace(str, @"[^\w\-а-я\-]", ""); // Keep Cyrillic letters
        return str;
    }

    private string EscapeXml(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";

        return System.Security.SecurityElement.Escape(input);
    }

    // Define your model classes below or reuse your existing ones

    public class FeedModel
    {
        public List<Product> Products { get; set; }
    }

    public class Product
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductPic { get; set; }
        public string Category { get; set; }
        public decimal PriceVat { get; set; }
        public decimal PriceDiscounted { get; set; }
    }
}
