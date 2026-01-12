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
            return GetMovies().FirstOrDefault(m => m.Id == id);
        }

        public List<Movie> GetMovies(string? searchString = null, string? genre = null, int skip = 0, int take = 100)
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

            return query.Skip(skip).Take(take).ToList();
        }

        public List<string> GetGenres()
        {
            var movies = GetMovies();
            return movies.SelectMany(m => m.Genre.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                         .Distinct()
                         .OrderBy(g => g)
                         .ToList();
        }
    }
}
