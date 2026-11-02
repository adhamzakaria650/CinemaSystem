using CinemaSystem.Data;
using CinemaSystem.Models;
using CinemaSystem.Repositories;
using CinemaSystem.ViewModels.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepo<Movie> _MovieRepo = new Repo<Movie>(); 
        private readonly IRepo<Cinema> _cinemaRepo = new Repo<Cinema>(); 
        private readonly IRepo<Actor> _actorRepo = new Repo<Actor>(); 
        private readonly IRepo<Category> _categoryRepo = new Repo<Category>(); 
        public IActionResult Index()
        {
            var viewmodel = new DashboardVM()
            {
                TotalMovies = _MovieRepo.GetAll().Count(),
                TotalCinemas = _cinemaRepo.GetAll().Count(),
                TotalActors = _actorRepo.GetAll().Count(),
                TotalCategories = _categoryRepo.GetAll().Count(),
                ActiveMovies = _MovieRepo.GetAll().Count(m => m.Status == true),
                InactiveMovies = _MovieRepo.GetAll().Count(m => m.Status == false),
                MoviesPerCategory = _categoryRepo.GetAll().AsQueryable()
                    .Select(c => new ChartItem
                    {
                        Label = c.Name,
                        Value = c.movies.Count 
                    }).ToList(),
                MoviesPerCinema = _cinemaRepo.GetAll().AsQueryable()
                    .Select(c => new ChartItem
                    {
                        Label = c.Name,
                        Value = c.movies.Count
                    }).ToList(),
                TopActors = _actorRepo.GetAll().AsQueryable()
                    .Select(a => new ChartItem
                    {
                        Label = a.Name,
                        Value = a.actorMovies.Count 
                    })
                    .OrderByDescending(x => x.Value)
                    .Take(5) 
                    .ToList()
            };
            return View(viewmodel);
        }
    }
}
