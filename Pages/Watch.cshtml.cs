using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieApp.Models;
using MovieApp.Services;

namespace MovieApp.Pages
{
    public class WatchModel : PageModel
    {
        private readonly MovieService _movieService;

        public Movie? Movie { get; set; }

        public WatchModel(MovieService movieService)
        {
            _movieService = movieService;
        }

        public IActionResult OnGet(int id)
        {
            Movie = _movieService.GetMovie(id);
            if (Movie == null)
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }
    }
}
