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

        public List<Movie> Movies { get; set; } = new();

        public IndexModel(ILogger<IndexModel> logger, MovieService movieService)
        {
            _logger = logger;
            _movieService = movieService;
        }

        public void OnGet()
        {
            Movies = _movieService.GetMovies(SearchString);
        }
    }
}
