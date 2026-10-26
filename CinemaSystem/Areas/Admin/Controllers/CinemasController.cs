using CinemaSystem.Data;
using CinemaSystem.Models;
using CinemaSystem.ViewModels.Cinema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemasController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(CinemaFilterVM c)
        {
            var cinemas = _context.cinemas.ToList();
            if (c.Name != null)
            {
                cinemas = cinemas.Where(x => x.Name.Contains(c.Name)).ToList();
            }
            if(c.Status==true || c.Status==false)
            {
                cinemas = cinemas.Where(x => x.Status == c.Status).ToList();
            }
            ViewBag.totalPages = (int)Math.Ceiling((double)cinemas.Count() / 5);
            cinemas = cinemas.Skip((c.PageNumber - 1) * 5).Take(5).ToList();
            ViewBag.currentPage = c.PageNumber;            
            return View(cinemas);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateCinemaVM c)
        {
            var fileName = ""; 
            if (c.Image != null && c.Image.Length > 0)
            {
                fileName = Guid.NewGuid().ToString() + "-" + c.Image.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\",fileName);
                //D:\VS projects\CinemaSystem\CinemaSystem\wwwroot\images\
                using(var streem = System.IO.File.Create(filePath))
                {
                    c.Image.CopyTo(streem);
                }
            }
            if (c.Name != null && c.Name.Length > 0)
            {
                _context.cinemas.Add(new Cinema()
                {
                    Name = c.Name,
                    Description = c.Description,
                    Status = c.Status,
                    Image = fileName
                });
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Update(int Id)
        {
            var cinema = _context.cinemas.FirstOrDefault(c=>c.Id==Id);
            return View(cinema);
        }
        [HttpPost]
        public IActionResult Update(UpdateCinemaVM c)
        {
            var oldCinema = _context.cinemas.AsNoTracking().FirstOrDefault(ci => ci.Id == c.Id);
            var fileName = "";
            if (c.Image != null && c.Image.Length > 0)
            {
                fileName = Guid.NewGuid().ToString() + "-" + c.Image.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\",fileName);
                using(var streem = System.IO.File.Create(filePath))
                {
                    c.Image.CopyTo(streem);
                }
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\",oldCinema.Image);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            else
            {
                fileName = oldCinema.Image;
            }
            if (c.Name != null && c.Name.Length > 0)
            {
                _context.cinemas.Update(new Cinema()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description=c.Description,
                    Status=c.Status,
                    Image = fileName
                });
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int Id)
        {
            var cinema = _context.cinemas.FirstOrDefault(c=>c.Id == Id);
            if (cinema != null)
            {
                var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", cinema.Image);
                if (System.IO.File.Exists(FilePath))
                {
                    System.IO.File.Delete(FilePath);
                }
                _context.cinemas.Remove(cinema);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}