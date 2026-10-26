namespace CinemaSystem.ViewModels.Cinema
{
    public class CinemaFilterVM
    {
        public string Name { get; set; }
        public bool? Status { get; set; } = null;
        public int PageNumber { get; set; } = 1;
    }
}
