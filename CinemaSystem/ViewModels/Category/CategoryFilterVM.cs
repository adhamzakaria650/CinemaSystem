namespace CinemaSystem.ViewModels.Category
{
    public class CategoryFilterVM
    {
        public string Name { get; set; }
        public bool? Status { get; set; }
        public int PageNumber { get; set; } = 1;
    }
}
