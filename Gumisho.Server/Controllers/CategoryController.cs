using Backend.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
 
    public class CategoryController : ControllerBase
    {
        // In-memory map (replace with DB access later)
        private static readonly Dictionary<string, List<string>> CategoryMap = new()
        {
            { "Кухня", new List<string> { "кухненски", "готварски", "съдове" } },
            { "Детски", new List<string> { "бебешки", "детски", "деца" } },
            { "Спорт", new List<string> { "спортен", "фитнес", "велосипед" } },
            { "Градина", new List<string> { "градински", "поливане", "навън" } },
        };
        private static readonly Regex CyrillicWordRegex = new(@"^\p{IsCyrillic}+$", RegexOptions.Compiled);

        [HttpPost("cloud")]
         

        public IActionResult GetUniqueWords([FromBody] string[] sentences)
        {
            var wordCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var sentence in sentences)
            {
                var words = Regex.Matches(sentence, @"\b\p{L}+\b")
                                 .Select(m => m.Value.ToLowerInvariant())
                                 .Where(w => w.Length > 3)
                                 .Where(w => CyrillicWordRegex.IsMatch(w));

                foreach (var word in words)
                {
                    if (wordCounts.ContainsKey(word))
                        wordCounts[word]++;
                    else
                        wordCounts[word] = 1;
                }
            }

            // Sort words by frequency descending and get only the words
            var sortedUniqueWords = wordCounts
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .ToArray();

            return Ok(sortedUniqueWords);
        }




        [HttpPost("map")]
        public IActionResult Mappings([FromBody] MappingDTO dto)
        {
            if (dto?.Words == null || dto?.Categories == null)
                return BadRequest("Words and Categories are required");

            var result = new Dictionary<string, List<string>>();
            var assignedCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var word in dto.Words)
            {
                // Find all categories that contain this word (case-insensitive)
                var matchingCategories = dto.Categories
                    .Where(cat => 
                        cat != null
                        && !assignedCategories.Contains(cat)         // Skip already assigned categories
                        && cat.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                // Mark these categories as assigned
                foreach (var cat in matchingCategories)
                {
                    assignedCategories.Add(cat);
                }

                result[word] = matchingCategories;
            }

            return Ok(result);
        }


        // GET api/CategoryMapping/all
        [HttpGet("all")]
        public IActionResult GetAllMappings()
        {
            return Ok(CategoryMap);
        }

        // GET api/CategoryMapping/map-word/детски
        [HttpGet("map-word/{word}")]
        public IActionResult MapWord(string word)
        {
            string lowerWord = word.ToLower();

            var result = CategoryMap
                .Where(kv => kv.Value.Contains(lowerWord))
                .Select(kv => kv.Key)
                .Distinct()
                .ToList();

            if (result.Count == 0)
                return NotFound("No matching category.");

            return Ok(result);
        }

        // POST api/CategoryMapping/add
        [HttpPost("add")]
        public IActionResult AddMapping([FromBody] CategoryMappingInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Category) || string.IsNullOrWhiteSpace(input.Word))
                return BadRequest("Category and word are required.");

            string cat = input.Category.Trim();
            string word = input.Word.Trim().ToLower();

            if (!CategoryMap.ContainsKey(cat))
                CategoryMap[cat] = new List<string>();

            if (!CategoryMap[cat].Contains(word))
                CategoryMap[cat].Add(word);

            return Ok(CategoryMap);
        }

        // DTO
        public class CategoryMappingInput
        {
            public string Category { get; set; }
            public string Word { get; set; }
        }
    }
}
