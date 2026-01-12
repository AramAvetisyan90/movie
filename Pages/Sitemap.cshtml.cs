using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieApp.Services;
using System.Text;

namespace MovieApp.Pages
{
    public class SitemapModel : PageModel
    {
        private readonly MovieService _movieService;

        public SitemapModel(MovieService movieService)
        {
            _movieService = movieService;
        }

        public IActionResult OnGet()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var movies = _movieService.GetMovies(take: int.MaxValue);

            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            // Homepage
            sb.AppendLine("  <url>");
            sb.AppendLine($"    <loc>{baseUrl}/</loc>");
            sb.AppendLine($"    <lastmod>{DateTime.Now:yyyy-MM-dd}</lastmod>");
            sb.AppendLine("    <changefreq>daily</changefreq>");
            sb.AppendLine("    <priority>1.0</priority>");
            sb.AppendLine("  </url>");

            // Movies
            foreach (var movie in movies)
            {
                var slug = _movieService.GetSlug(movie.Title);
                sb.AppendLine("  <url>");
                sb.AppendLine($"    <loc>{baseUrl}/Watch/{slug}</loc>");
                sb.AppendLine($"    <lastmod>{DateTime.Now:yyyy-MM-dd}</lastmod>");
                sb.AppendLine("    <changefreq>weekly</changefreq>");
                sb.AppendLine("    <priority>0.8</priority>");
                sb.AppendLine("  </url>");
            }

            sb.AppendLine("</urlset>");

            return Content(sb.ToString(), "application/xml", Encoding.UTF8);
        }
    }
}
