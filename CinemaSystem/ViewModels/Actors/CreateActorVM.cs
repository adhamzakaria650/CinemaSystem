using Microsoft.Identity.Client;

namespace CinemaSystem.ViewModels.Actors
{
    public class CreateActorVM
    {
        public string Name { get; set; }
        public IFormFile Image { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}
