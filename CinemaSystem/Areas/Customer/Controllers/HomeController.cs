using CinemaSystem.Data;
using CinemaSystem.ViewModels.Cinema;
using CinemaSystem.ViewModels.Movies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

namespace CinemaSystem.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(string Name)
        {
            var movies = _context.movies.AsQueryable();
            movies = movies.Include(m => m.category).Include(m=>m.cinema);
            if (Name != null && Name.Length > 0)
            {
                movies = movies.Where(m=>m.Name.Contains(Name));
            }
            return View(movies.ToList());
        }
        public IActionResult ShowCinemas()
        {
            var cinemas = _context.cinemas.AsQueryable();
            return View(cinemas.ToList());
        }
        public IActionResult ShowMovieDetails(int id)
        {
            var movie = _context.movies.Include(m=>m.category).Include(m=>m.cinema).FirstOrDefault(m => m.Id == id);
            var movieSubimages = _context.movie_subimages.Where(si => si.MovieId == id).ToList();
            var movieactors = _context.actor_movies.Where(a => a.MovieID == id).Include(a=>a.actor).ToList();
            return View(new ShowMovieDetailsVM()
            {
                movie = movie,
                movieSubImages = movieSubimages,
                actorMovies = movieactors
            });

        }
        public IActionResult ShowCinemaMovies(int id)
        {
            var cinema = _context.cinemas.FirstOrDefault(c=>c.Id==id);
            var movies = _context.movies.Where(m => m.cinemaId == id);

            return View(new ShowCinemaMoviesVM()
            {
                Cinema  = cinema,
                movies = movies.ToList()

            });
        }
    }
}
