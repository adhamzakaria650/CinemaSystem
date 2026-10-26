using CinemaSystem.Data;
using CinemaSystem.Models;
using Microsoft.AspNetCore.Mvc;
using CinemaSystem.ViewModels.Category;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(CategoryFilterVM c)
        {
            var categories = _context.categories.ToList();
            if (c.Name != null)
            {
                categories = categories.Where(x => x.Name.Contains(c.Name)).ToList();
            }
            if(c.Status==true || c.Status==false)
            {
                categories = categories.Where(x => x.Status == c.Status).ToList();
            }
            ViewBag.totalPages = (int)Math.Ceiling((double)categories.Count() / 5);
            categories = categories.Skip((c.PageNumber - 1) * 5).Take(5).ToList();
            ViewBag.currentPage = c.PageNumber;            
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateCategoryVM c)
        {

            if (c.Name != null && c.Name.Length > 0)
            {
                _context.categories.Add(new Category()
                {
                    Name = c.Name,
                    Description = c.Description,
                    Status = c.Status
                });
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Update(int Id)
        {
            var category = _context.categories.FirstOrDefault(c=>c.Id==Id);
            return View(category);
        }
        [HttpPost]
        public IActionResult Update(UpdateCategoryVM c)
        {
            if (c.Name != null && c.Name.Length > 0)
            {
                _context.categories.Update(new Category()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description=c.Description,
                    Status=c.Status
                });
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int Id)
        {
            var category = _context.categories.FirstOrDefault(c=>c.Id == Id);
            if (category != null)
            {
                _context.categories.Remove(category);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}