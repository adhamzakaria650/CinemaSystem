namespace CinemaSystem.ViewModels.Actors
{
    public class ActorFilterVM
    {
        public string Name { get; set; }
        public bool? Status { get; set; } = null;

        public int PageNumber { get; set; } = 1;
    }
}
