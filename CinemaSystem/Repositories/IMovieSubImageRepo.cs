using CinemaSystem.Models;

namespace CinemaSystem.Repositories
{
    public interface IMovieSubImageRepo : IRepo<MovieSubImage>
    {
        void RemoveAll(IEnumerable<MovieSubImage> subImages);
    }
}
