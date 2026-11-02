using CinemaSystem.Data;
using CinemaSystem.Models;

namespace CinemaSystem.Repositories
{
    public class ActorMovieRepo :Repo<ActorMovie>,IMovieActorsRepo
    {
        public ActorMovieRepo(ApplicationDbContext context):base(context)
        {

        }
        public void RemoveAll(IEnumerable<ActorMovie> actorMovies)
        {
            _dbset.RemoveRange(actorMovies);
        }
    }
}