using CinemaSystem.Models;

namespace CinemaSystem.Repositories
{
    public class ActorMovieRepo :Repo<ActorMovie>,IMovieActorsRepo
    {
        public void RemoveAll(IEnumerable<ActorMovie> actorMovies)
        {
            _dbset.RemoveRange(actorMovies);
        }
    }
}