using CinemaSystem.Models;

namespace CinemaSystem.ViewModels.Actors
{
    public class ActorVM
    {
        public List<Actor> Actors { get; set; }
        public List<ActorMovie> actorMovies { get; set; }
    }
}
