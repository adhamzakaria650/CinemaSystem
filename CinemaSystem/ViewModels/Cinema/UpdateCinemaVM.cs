namespace CinemaSystem.ViewModels.Cinema
{
    public class UpdateCinemaVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public IFormFile Image { get; set; }
    }
}
