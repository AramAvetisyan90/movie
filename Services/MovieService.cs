using MovieApp.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text.Json;

namespace MovieApp.Services
{
    public class MovieService
    {
        private readonly IConfiguration _configuration;

        public MovieService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Movie? GetMovie(int id)
        {
            return GetMovies(take: int.MaxValue).FirstOrDefault(m => m.Id == id);
        }

        public Movie? GetMovieByTitle(string slug)
        {
            var allMovies = GetMovies(take: int.MaxValue);
            return allMovies.FirstOrDefault(m => GetSlug(m.Title).Equals(slug, StringComparison.OrdinalIgnoreCase));
        }

        public string GetSlug(string title)
        {
            string transliterated = Transliterate(title.ToLower());
            // Remove special characters and replace spaces with hyphens
            string slug = System.Text.RegularExpressions.Regex.Replace(transliterated, @"[^a-z0-9\s-]", "");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-").Trim('-');
            return slug;
        }

        private string Transliterate(string text)
        {
            var words = new Dictionary<string, string>
            {
                {"а", "a"}, {"б", "b"}, {"в", "v"}, {"г", "g"}, {"д", "d"}, {"е", "e"}, {"ё", "yo"},
                {"ж", "zh"}, {"з", "z"}, {"и", "i"}, {"й", "y"}, {"к", "k"}, {"л", "l"}, {"м", "m"}, {"н", "n"},
                {"о", "o"}, {"п", "p"}, {"р", "r"}, {"с", "s"}, {"т", "t"}, {"у", "u"}, {"ф", "f"}, {"х", "kh"},
                {"ц", "ts"}, {"ч", "ch"}, {"ш", "sh"}, {"щ", "shch"}, {"ъ", ""}, {"ы", "y"}, {"ь", ""}, {"э", "e"},
                {"ю", "yu"}, {"я", "ya"}
            };
            foreach (var item in words)
            {
                text = text.Replace(item.Key, item.Value);
            }
            return text;
        }

        public List<Movie> GetMovies(string? searchString = null, string? genre = null, string? country = null, int? year = null, int skip = 0, int take = 100)
        {
            var movies = new List<Movie>();
            var basePath = _configuration["MediaSettings:BasePath"] ?? "/var/www/MovieData";
            var jsonPath = Path.Combine(basePath, "movies.json");
            
            if (File.Exists(jsonPath))
            {
                try
                {
                    var json = File.ReadAllText(jsonPath);
                    movies = JsonSerializer.Deserialize<List<Movie>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Movie>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MovieService] Error loading movies: {ex.Message}");
                }
            }

            var query = movies.AsEnumerable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(m => m.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(genre) && genre != "All")
            {
                query = query.Where(m => 
                    !string.IsNullOrEmpty(m.Genre) && 
                    m.Genre.Split(',', StringSplitOptions.TrimEntries)
                           .Contains(genre, StringComparer.OrdinalIgnoreCase)
                );
            }

            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(m => 
                    !string.IsNullOrEmpty(m.Country) && 
                    m.Country.Split(',', StringSplitOptions.TrimEntries)
                           .Contains(country, StringComparer.OrdinalIgnoreCase)
                );
            }

            if (year.HasValue && year > 0)
            {
                query = query.Where(m => m.ReleaseYear == year.Value);
            }

            return query.Skip(skip).Take(take).ToList();
        }

        public List<string> GetGenres()
        {
            var movies = GetMovies(take: int.MaxValue);
            return movies.SelectMany(m => m.Genre.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                         .Distinct()
                         .OrderBy(g => g)
                         .ToList();
        }

        public List<string> GetCountries()
        {
            var movies = GetMovies(take: int.MaxValue);
            return movies.SelectMany(m => m.Country.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                         .Distinct()
                         .OrderBy(c => c)
                         .ToList();
        }

        public List<int> GetYears()
        {
            var movies = GetMovies(take: int.MaxValue);
            return movies.Select(m => m.ReleaseYear)
                         .Distinct()
                         .OrderByDescending(y => y)
                         .ToList();
        }
    }
}
