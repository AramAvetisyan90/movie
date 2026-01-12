using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieApp.Models;
using MovieApp.Services;

namespace MovieApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly MovieService _movieService;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Genre { get; set; }

        public List<Movie> Movies { get; set; } = new();
        public List<string> AvailableGenres { get; set; } = new();

        public IndexModel(ILogger<IndexModel> logger, MovieService movieService)
        {
            _logger = logger;
            _movieService = movieService;
        }

        public void OnGet()
        {
            // Initial load of first 100 movies
            Movies = _movieService.GetMovies(SearchString, Genre, skip: 0, take: 100);
            AvailableGenres = _movieService.GetGenres();
        }

        public IActionResult OnGetMoreMovies(int skip, string? search, string? genre)
        {
            var moreMovies = _movieService.GetMovies(search, genre, skip: skip, take: 100);
            return new JsonResult(moreMovies);
        }
    }
}
