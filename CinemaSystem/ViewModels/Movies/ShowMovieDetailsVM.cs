namespace CinemaSystem.ViewModels.Movies
{
    public class ShowMovieDetailsVM
    {
        public Models.Movie movie { get; set; }
        public List<Models.MovieSubImage> movieSubImages { get; set; }
        public List<Models.ActorMovie> actorMovies { get; set; }
    }
}
