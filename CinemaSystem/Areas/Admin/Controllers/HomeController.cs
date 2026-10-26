using CinemaSystem.Data;
using CinemaSystem.ViewModels.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var viewmodel = new DashboardVM()
            {
                TotalMovies = _context.movies.Count(),
                TotalCinemas = _context.cinemas.Count(),
                TotalActors = _context.actors.Count(),
                TotalCategories = _context.categories.Count(),
                ActiveMovies = _context.movies.Count(m => m.Status == true),
                InactiveMovies = _context.movies.Count(m => m.Status == false),
                MoviesPerCategory = _context.categories
                    .Select(c => new ChartItem
                    {
                        Label = c.Name,
                        Value = c.movies.Count 
                    }).ToList(),
                MoviesPerCinema = _context.cinemas
                    .Select(c => new ChartItem
                    {
                        Label = c.Name,
                        Value = c.movies.Count
                    }).ToList(),
                TopActors = _context.actors
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
