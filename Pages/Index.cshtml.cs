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

        public List<Movie> Movies { get; set; } = new();
        public List<string> AvailableGenres { get; set; } = new();

        public MovieService Library { get; }

        public IndexModel(ILogger<IndexModel> logger, MovieService movieService)
        {
            _logger = logger;
            Library = movieService;
        }

        public void OnGet()
        {
            // Initial load of first 100 movies
            Movies = Library.GetMovies(SearchString, Genre, skip: 0, take: 100);
            AvailableGenres = Library.GetGenres();
        }

        public IActionResult OnGetMoreMovies(int skip, string? search, string? genre)
        {
            var moreMovies = Library.GetMovies(search, genre, skip: skip, take: 100);
            var results = moreMovies.Select(m => new {
                m.Id,
                m.Title,
                m.Description,
                m.PosterUrl,
                m.ReleaseYear,
                m.Genre,
                Slug = Library.GetSlug(m.Title)
            });
            return new JsonResult(results);
        }
    }
}
