namespace CinemaSystem.ViewModels.Movies
{
    public class MovieFilterVM
    {
        public string Name { get; set; }
        public double MinPrice { get; set; } = -1;
        public double MaxPrice { get; set; } = -1;
        public int CategoryId { get; set; } = -1;
        public int CinemaId { get; set; } = -1;
        public bool? Status { get; set; } = null;
        public int PageNumber { get; set; } = 1;
    }
}
