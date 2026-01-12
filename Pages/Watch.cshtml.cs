using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.IO;
using MovieApp.Models;
using MovieApp.Services;

namespace MovieApp.Pages
{
    public class WatchModel : PageModel
    {
        private readonly MovieService _movieService;
        private readonly IConfiguration _configuration;

        public Movie? Movie { get; set; }

        public WatchModel(MovieService movieService, IConfiguration configuration)
        {
            _movieService = movieService;
            _configuration = configuration;
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

        public IActionResult OnGetVideo(int id)
        {
            var movie = _movieService.GetMovie(id);
            if (movie == null) return NotFound();

            var basePath = _configuration["MediaSettings:BasePath"] ?? "C:\\MovieData";
            
            // Clean up the VideoUrl from the JSON
            var videoUrl = movie.VideoUrl.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
            
            // If the VideoUrl already starts with "videos", don't prepend it again (depends on JSON structure)
            // If it doesn't, we might need to handle it. Let's try to find the file in both places.
            string fullPath = Path.Combine(basePath, videoUrl);
            
            if (!System.IO.File.Exists(fullPath))
            {
                // Try prepending 'videos' if it's not there
                if (!videoUrl.StartsWith("videos", StringComparison.OrdinalIgnoreCase))
                {
                    fullPath = Path.Combine(basePath, "videos", videoUrl);
                }
            }

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound($"Video file not found at: {fullPath}");
            }

            return new PhysicalFileResult(fullPath, "video/mp4")
            {
                EnableRangeProcessing = true
            };
        }
    }
}
