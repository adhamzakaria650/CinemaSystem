namespace CinemaSystem.ViewModels.Cinema
{
    public class CreateCinemaVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public IFormFile Image { get; set; }
    }
}
