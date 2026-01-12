namespace MovieApp.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
    }
}
