using CinemaSystem.Data;
using CinemaSystem.Models;
using CinemaSystem.ViewModels.Actors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace ActorSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(ActorFilterVM a)
        {
            var actors = _context.actors.ToList();
            var ActorMovies = _context.actor_movies.Include(am => am.movie).ToList(); 
            if (a.Name != null)
            {
                actors = actors.Where(x => x.Name.Contains(a.Name)).ToList();
            }
            if(a.Status==true || a.Status==false)
            {
                actors = actors.Where(x => x.Status == a.Status).ToList();
            }
            ViewBag.totalPages = (int)Math.Ceiling((double)actors.Count() / 5);
            actors = actors.Skip((a.PageNumber - 1) * 5).Take(5).ToList();
            ViewBag.currentPage = a.PageNumber;            
            return View(new ActorVM()
            {
                Actors = actors,
                actorMovies = ActorMovies
            });
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateActorVM a)
        {
            var fileName = ""; 
            if (a.Image != null && a.Image.Length > 0)
            {
                fileName = Guid.NewGuid().ToString() + "-" + a.Image.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\",fileName);
                //D:\VS projects\ActorSystem\ActorSystem\wwwroot\images\
                using(var streem = System.IO.File.Create(filePath))
                {
                    a.Image.CopyTo(streem);
                }
            }
            if (a.Name != null && a.Name.Length > 0)
            {
                _context.actors.Add(new Actor()
                {
                    Name = a.Name,
                    Description = a.Description,
                    Status = a.Status,
                    Image = fileName
                });
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int Id)
        {
            var actor = _context.actors.FirstOrDefault(a=>a.Id==Id);
            return View(actor);
        }
        [HttpPost]
        public IActionResult Update(UpdateActorVM a)
        {
            var oldActor = _context.actors.AsNoTracking().FirstOrDefault(ci => ci.Id == a.Id);
            var fileName = "";
            if (a.Image != null && a.Image.Length > 0)
            {
                fileName = Guid.NewGuid().ToString() + "-" + a.Image.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\",fileName);
                using(var streem = System.IO.File.Create(filePath))
                {
                    a.Image.CopyTo(streem);
                }
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\",oldActor.Image);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            else
            {
                fileName = oldActor.Image;
            }
            if (a.Name != null && a.Name.Length > 0)
            {
                _context.actors.Update(new Actor()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description=a.Description,
                    Status=a.Status,
                    Image = fileName
                });
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int Id)
        {
            var actor = _context.actors.FirstOrDefault(a=>a.Id == Id);
            if (actor != null)
            {
                var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", actor.Image);
                if (System.IO.File.Exists(FilePath))
                {
                    System.IO.File.Delete(FilePath);
                }
                _context.actors.Remove(actor);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}