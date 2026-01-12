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

        public List<Movie> GetMovies(string? searchString = null)
        {
            var movies = new List<Movie>();
            var basePath = _configuration["MediaSettings:BasePath"] ?? "/var/www/MovieData";
            var jsonPath = Path.Combine(basePath, "movies.json");
            
            // Log for debugging
            Console.WriteLine($"[MovieService] Looking for movies.json at: {jsonPath}");
            Console.WriteLine($"[MovieService] File exists: {File.Exists(jsonPath)}");
            
            if (File.Exists(jsonPath))
            {
                try
                {
                    var json = File.ReadAllText(jsonPath);
                    Console.WriteLine($"[MovieService] JSON content length: {json.Length} characters");
                    movies = JsonSerializer.Deserialize<List<Movie>>(json) ?? new List<Movie>();
                    Console.WriteLine($"[MovieService] Loaded {movies.Count} movies");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MovieService] Error loading movies: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"[MovieService] movies.json not found at {jsonPath}");
                Console.WriteLine($"[MovieService] Current directory: {Directory.GetCurrentDirectory()}");
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(m => m.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return movies;
        }
    }
}
