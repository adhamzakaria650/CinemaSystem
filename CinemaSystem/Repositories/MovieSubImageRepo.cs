using CinemaSystem.Models;

namespace CinemaSystem.Repositories
{
    public class MovieSubImageRepo: Repo<MovieSubImage> , IMovieSubImageRepo
    {
        public void RemoveAll(IEnumerable<MovieSubImage> subImages)
        {
            _dbset.RemoveRange(subImages);
        }
    }
}
