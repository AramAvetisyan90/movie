using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieApp.Models;
using MovieApp.Services;

namespace MovieApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Genre { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Country { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Year { get; set; }

        public List<Movie> Movies { get; set; } = new();
        public List<string> AvailableGenres { get; set; } = new();
        public List<string> AvailableCountries { get; set; } = new();
        public List<int> AvailableYears { get; set; } = new();

        public MovieService Library { get; }

        public IndexModel(ILogger<IndexModel> logger, MovieService movieService)
        {
            _logger = logger;
            Library = movieService;
        }

        public void OnGet()
        {
            // Initial load of first 100 movies
            Movies = Library.GetMovies(SearchString, Genre, Country, Year, skip: 0, take: 100);
            AvailableGenres = Library.GetGenres();
            AvailableCountries = Library.GetCountries();
            AvailableYears = Library.GetYears();
        }

        public IActionResult OnGetMoreMovies(int skip, string? search, string? genre, string? country, int? year)
        {
            var moreMovies = Library.GetMovies(search, genre, country, year, skip: skip, take: 100);
            var results = moreMovies.Select(m => new {
                m.Id,
                m.Title,
                m.Description,
                m.PosterUrl,
                m.ReleaseYear,
                m.Genre,
                m.Country,
                m.Duration,
                Slug = Library.GetSlug(m.Title)
            });
            return new JsonResult(results);
        }
    }
}
