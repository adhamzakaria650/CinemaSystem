using CinemaSystem.Data;
using CinemaSystem.Models;
using CinemaSystem.Repositories;
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
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepo<Movie> _movieRepo = new Repo<Movie>();
        private readonly IRepo<Cinema> _cinemaRepo = new Repo<Cinema>(); 
        private readonly IRepo<MovieSubImage> _movieSubImageRepo = new Repo<MovieSubImage>(); 
        private readonly IRepo<ActorMovie> _ActorMovieRepo = new Repo<ActorMovie>(); 
        public IActionResult Index(string Name)
        {
            //var movies = _context.movies.AsQueryable();
            //movies = movies.Include(m => m.category).Include(m=>m.cinema);
            var movies = _movieRepo.GetAll(Includes: [m => m.category, m => m.cinema]).AsQueryable();
            if (Name != null && Name.Length > 0)
            {
                movies = movies.Where(m=>m.Name.Contains(Name));
            }
            return View(movies.ToList());
        }
        public IActionResult ShowCinemas()
        {
            //var cinemas = _context.cinemas.AsQueryable();
            var cinemas = _cinemaRepo.GetAll();
            return View(cinemas.ToList());
        }
        public async Task<IActionResult> ShowMovieDetails(int id)
        {
            //var movie = _context.movies.Include(m=>m.category).Include(m=>m.cinema).FirstOrDefault(m => m.Id == id);
            var movie = await _movieRepo.GetOneAsync(Includes: [m => m.category, m => m.cinema]);
            //var movieSubimages = _context.movie_subimages.Where(si => si.MovieId == id).ToList();
            var movieSubimages = _movieSubImageRepo.GetAll(si => si.MovieId == id).ToList();
            //var movieactors = _context.actor_movies.Where(a => a.MovieID == id).Include(a=>a.actor).ToList();
            var movieactors = _ActorMovieRepo.GetAll(a => a.MovieID == id, Includes: [a => a.actor]).ToList();
            return View(new ShowMovieDetailsVM()
            {
                movie = movie,
                movieSubImages = movieSubimages,
                actorMovies = movieactors
            });

        }
        public async Task<IActionResult> ShowCinemaMovies(int id)
        {
            //var cinema = _context.cinemas.FirstOrDefault(c=>c.Id==id);
            var cinema = await _cinemaRepo.GetOneAsync(c => c.Id == id);
            //var movies = _context.movies.Where(m => m.cinemaId == id);
            var movies = _movieRepo.GetAll(m => m.cinemaId == id);
            return View(new ShowCinemaMoviesVM()
            {
                Cinema  = cinema,
                movies = movies.ToList()

            });
        }
    }
}
