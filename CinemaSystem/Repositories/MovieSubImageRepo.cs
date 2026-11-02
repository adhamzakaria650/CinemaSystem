using CinemaSystem.Data;
using CinemaSystem.Models;

namespace CinemaSystem.Repositories
{
    public class MovieSubImageRepo: Repo<MovieSubImage> , IMovieSubImageRepo
    {
        public MovieSubImageRepo(ApplicationDbContext context) : base(context)
        {

        }
        public void RemoveAll(IEnumerable<MovieSubImage> subImages)
        {
            _dbset.RemoveRange(subImages);
        }
    }
}
