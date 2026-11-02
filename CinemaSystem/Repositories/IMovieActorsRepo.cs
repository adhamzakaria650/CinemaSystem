using CinemaSystem.Models;

namespace CinemaSystem.Repositories
{
    public interface IMovieActorsRepo : IRepo<ActorMovie>
    {
        void RemoveAll(IEnumerable<ActorMovie> actorMovies); 
    }
}
