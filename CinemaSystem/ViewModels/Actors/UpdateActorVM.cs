namespace CinemaSystem.ViewModels.Actors
{
    public class UpdateActorVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IFormFile Image { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}
