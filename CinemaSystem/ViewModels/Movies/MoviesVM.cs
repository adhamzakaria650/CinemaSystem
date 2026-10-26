using CinemaSystem.Models;
namespace CinemaSystem.ViewModels.Movie
{
    public class MoviesVM
    {
        public IEnumerable<Models.Movie> movies { get; set; }
        public IEnumerable<Models.ActorMovie> actorMovies { get; set; }
        public IEnumerable<Models.Category> categories { get; set; }
        public IEnumerable<Models.Cinema> cinemas{ get; set; }
        
        public int PageNumber { get; set; } = 1;
    }
}
