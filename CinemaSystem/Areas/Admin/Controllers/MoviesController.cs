using CinemaSystem.Data;
using CinemaSystem.Models;
using CinemaSystem.Repositories;
using CinemaSystem.ViewModels.Movie;
using CinemaSystem.ViewModels.Movies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace MovieSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MoviesController : Controller
    {
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepo<Movie> _movieRepo;//= new Repo<Movie>();
        private readonly IMovieActorsRepo _ActorMovieRepo;//= new ActorMovieRepo();
        private readonly IRepo<Actor> _ActorRepo;//= new Repo<Actor>();
        private readonly IRepo<Category> _CategoryRepo;//= new Repo<Category>();
        private readonly IRepo<Cinema> _CinemaRepo;//= new Repo<Cinema>();
        private readonly IMovieSubImageRepo _MovieSubImageRepo;//= new MovieSubImageRepo();

        public MoviesController(IRepo<Movie> movieRepo, IMovieActorsRepo actorMovieRepo, IRepo<Actor> actorRepo, IRepo<Category> categoryRepo, IRepo<Cinema> cinemaRepo, IMovieSubImageRepo movieSubImageRepo)
        {
            _movieRepo = movieRepo;
            _ActorMovieRepo = actorMovieRepo;
            _ActorRepo = actorRepo;
            _CategoryRepo = categoryRepo;
            _CinemaRepo = cinemaRepo;
            _MovieSubImageRepo = movieSubImageRepo;
        }
        public IActionResult Index(MovieFilterVM c)
        {
            //var movies = _context.movies.AsQueryable();
            //movies = movies.Include(m => m.category);
            //movies = movies.Include(m => m.cinema);
            var movies = _movieRepo.GetAll(Includes: [m => m.category, m => m.cinema]).AsQueryable();
            //var actor_movies = _context.actor_movies.AsQueryable();
            //actor_movies = actor_movies.Include(a => a.movie).Include(a=>a.actor);
            var actor_movies = _ActorMovieRepo.GetAll(Includes: [a => a.movie, a => a.actor]).AsQueryable();

            if (c.Name != null)
            {
                movies = movies.Where(x => x.Name.Contains(c.Name));
            }
            if (c.MinPrice != -1 && c.MaxPrice == -1)
            {
                movies = movies.Where(m => m.Price >= c.MinPrice);
            }
            if (c.MaxPrice != -1 && c.MinPrice == -1)
            {
                movies = movies.Where(m => m.Price <= c.MaxPrice);
            }
            if (c.MaxPrice != -1 && c.MinPrice != -1)
            {
                movies = movies.Where(m => m.Price >= c.MinPrice && m.Price <= c.MaxPrice);
            }
            if (c.CategoryId != -1)
            {
                movies = movies.Where(m => m.categoryId == c.CategoryId);
            }
            if (c.CinemaId != -1)
            {
                movies = movies.Where(m => m.cinemaId == c.CinemaId);
            }
            if (c.Status == true || c.Status == false)
            {
                movies = movies.Where(x => x.Status == c.Status);
            }
            ViewBag.totalPages = (int)Math.Ceiling((double)movies.Count() / 5);
            movies = movies.Skip((c.PageNumber - 1) * 5).Take(5);
            ViewBag.currentPage = c.PageNumber;
            return View(new MoviesVM()
            {
                movies = movies.AsEnumerable(),
                actorMovies = actor_movies.ToList(),
                //categories = _context.categories.AsEnumerable(),
                categories = _CategoryRepo.GetAll(),
                //cinemas = _context.cinemas.AsEnumerable(),
                cinemas = _CinemaRepo.GetAll(),
            });
        }
        [HttpGet]
        public IActionResult Create()
        {
            //var Categories = _context.categories.ToList();
            var Categories = _CategoryRepo.GetAll().ToList();
            //var Actors = _context.actors.ToList();
            var Actors = _ActorRepo.GetAll().ToList();
            //var Cinemas = _context.cinemas.ToList();
            var Cinemas = _CinemaRepo.GetAll().ToList();

            ViewBag.Actors = Actors;
            ViewBag.Categories = Categories;
            ViewBag.Cinemas = Cinemas;
            return View(new CreateMovieVM());
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateMovieVM c)
        {
            if (!ModelState.IsValid)
            {
                //var Categories = _context.categories.ToList();
                var Categories = _CategoryRepo.GetAll().ToList();
                //var Actors = _context.actors.ToList();
                var Actors = _ActorRepo.GetAll().ToList();
                //var Cinemas = _context.cinemas.ToList();
                var Cinemas = _CinemaRepo.GetAll().ToList();
                ViewBag.Actors = Actors;
                ViewBag.Categories = Categories;
                ViewBag.Cinemas = Cinemas;
                return View(c);
            }
            var MainImageName = "";
            var SubImagesName = "";
            int MovieId;
            if (c.MainImage != null && c.MainImage.Length > 0)
            {
                MainImageName = Guid.NewGuid().ToString() + "-" + c.MainImage.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", MainImageName);
                //D:\VS projects\MovieSystem\MovieSystem\wwwroot\images\
                using (var streem = System.IO.File.Create(filePath))
                {
                    c.MainImage.CopyTo(streem);
                }
            }
            if (c.Name != null && c.Name.Length > 0)
            {
                var movie= await _movieRepo.AddAsync(new Movie()
                {
                    Name = c.Name,
                    Description = c.Description,
                    Status = c.Status,
                    Price = c.Price,
                    Time = c.Time,
                    categoryId = c.CategoryId,
                    cinemaId = c.CinemaId,
                    MainImg= MainImageName
                });
                await _movieRepo.CommitAsync();
                MovieId = movie.Entity.Id;
                if (c.SubImages != null && c.SubImages.Count() > 0)
                {
                    foreach (var image in c.SubImages)
                    {
                        if (image != null && image.Length > 0)
                        {
                            MainImageName = Guid.NewGuid().ToString() + "-" + image.FileName;
                            var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", MainImageName);
                            using (var streem = System.IO.File.Create(FilePath))
                            {
                                image.CopyTo(streem);
                            }
                            await _MovieSubImageRepo.AddAsync(new MovieSubImage()
                            {
                                MovieId = MovieId,
                                Image = MainImageName
                            });
                        }
                    }
                }
                if (c.ActorId != null && c.ActorId.Count() > 0)
                {
                    foreach(int actor in c.ActorId)
                    {
                        await _ActorMovieRepo.AddAsync(new ActorMovie()
                        {
                            ActorID = actor,
                            MovieID = MovieId
                        });
                    }
                }
            }
            //_context.SaveChanges();
            await _MovieSubImageRepo.CommitAsync();
            await _ActorMovieRepo.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int Id)
        {
            //var movie = _context.movies.FirstOrDefault(c => c.Id == Id);
            var movie = await _movieRepo.GetOneAsync(c => c.Id == Id);
            //var Actor_Movies = _context.actor_movies.Where(am=>am.MovieID==Id).AsQueryable();
            var Actor_Movies = _ActorMovieRepo.GetAll(am => am.MovieID == Id);
            //var actors = _context.actors.AsQueryable();
            var actors = _ActorRepo.GetAll();
            
            var categories = _CategoryRepo.GetAll();
            var cinemas = _CinemaRepo.GetAll();
            //var Subimages = _context.movie_subimages.Where(si=>si.MovieId==Id).AsQueryable();
            var Subimages = _MovieSubImageRepo.GetAll(si => si.MovieId == Id);

            ViewBag.Actor_Movies = Actor_Movies.ToList();
            ViewBag.Categories = categories.AsEnumerable();
            ViewBag.Cinemas = cinemas.AsEnumerable();
            ViewBag.SubImages = Subimages.AsEnumerable();
            ViewBag.Actors = actors.AsEnumerable();
            return View(movie);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UpdateMovieVM m)
        {
            //var oldMovie = _context.movies.AsNoTracking().FirstOrDefault(mo => mo.Id == m.Id);
            var oldMovie = await _movieRepo.GetOneAsync(mo => mo.Id == m.Id,isTracked:false);
            var fileName = "";
            if (m.MainImage != null && m.MainImage.Length > 0)
            {
                fileName = Guid.NewGuid().ToString() + "-" + m.MainImage.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", fileName);
                using (var streem = System.IO.File.Create(filePath))
                {
                    m.MainImage.CopyTo(streem);
                }
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", oldMovie.MainImg);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            else
            {
                fileName = oldMovie.MainImg;
            }
            if (m.Name != null && m.Name.Length > 0)
            {

                DateTime Date;
                if (m.Time != null)
                {
                    Date = (DateTime)m.Time;
                }
                else
                {
                    Date = oldMovie.Time;
                }
                var movie = _movieRepo.Update(new Movie()
                {
                    Id = m.Id,
                    Name = m.Name,
                    Price = m.Price,
                    categoryId = m.CategoryId,
                    cinemaId = m.CinemaId,
                    Description = m.Description,
                    Time = Date,
                    Status = m.Status,
                    MainImg = fileName
                });
                await _movieRepo.CommitAsync();
                if (m.SubImages != null && m.SubImages.Count() > 0)
                {
                    //var OldSubimages = _context.movie_subimages.Where(s => s.MovieId == m.Id);
                    var OldSubimages = _MovieSubImageRepo.GetAll(s => s.MovieId == m.Id);
                    if (OldSubimages != null)
                    {
                        foreach (var oldSubimage in OldSubimages)
                        {
                            var oldpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", oldSubimage.Image);
                            if (System.IO.File.Exists(oldpath))
                            {
                                System.IO.File.Delete(oldpath);
                            }
                        }
                        //_context.movie_subimages.RemoveRange(OldSubimages);
                        _MovieSubImageRepo.RemoveAll(OldSubimages);
                    }
                    var NewImageName = "";
                    foreach (var subimage in m.SubImages)
                    {
                        if (subimage != null && subimage.Length > 0)
                        {
                            NewImageName = Guid.NewGuid().ToString() + "-" + subimage.FileName;
                            var NewImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", NewImageName);
                            using(var stream = System.IO.File.Create(NewImagePath))
                            {
                                subimage.CopyTo(stream);
                            }
                            await _MovieSubImageRepo.AddAsync(new MovieSubImage()
                            {
                                MovieId = m.Id,
                                Image = NewImageName,
                            });
                        }
                    }
                }

                if (m.ActorId != null && m.ActorId.Count() > 0)
                {
                    //var oldActors = _context.actor_movies.Where(am=>am.MovieID == m.Id);
                    var oldActors = _ActorMovieRepo.GetAll(am => am.MovieID == m.Id);
                    //_context.actor_movies.RemoveRange(oldActors);
                    _ActorMovieRepo.RemoveAll(oldActors);
                    foreach (int actor in m.ActorId)
                    {
                        await _ActorMovieRepo.AddAsync(new ActorMovie()
                        {
                            MovieID = m.Id,
                            ActorID = actor
                        });
                    }
                }
            }
            //_context.SaveChanges();
            await _MovieSubImageRepo.CommitAsync();
            await _ActorMovieRepo.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int Id)
        {
            //var movie = _context.movies.FirstOrDefault(c => c.Id == Id);
            var movie = await _movieRepo.GetOneAsync(c => c.Id == Id);
            //var oldMovieSubImages = _context.movie_subimages.Where(msi => msi.MovieId == Id);
            var oldMovieSubImages = _MovieSubImageRepo.GetAll(msi => msi.MovieId == Id);
            //var oldActorsinThatMovie = _context.actor_movies.Where(ac => ac.MovieID == Id);
            var oldActorsinThatMovie = _ActorMovieRepo.GetAll(ac => ac.MovieID == Id);
            if (movie != null)
            {
                var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", movie.MainImg);
                if (System.IO.File.Exists(FilePath))
                {
                    System.IO.File.Delete(FilePath);
                }
                //_context.movies.Remove(movie);
                _movieRepo.Remove(movie);
                if (oldMovieSubImages != null && oldMovieSubImages.Count() > 0)
                {
                    foreach(var image in oldMovieSubImages)
                    {
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", image.Image);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                }
                if (oldActorsinThatMovie != null)
                {
                    //_context.actor_movies.RemoveRange(oldActorsinThatMovie);
                    _ActorMovieRepo.RemoveAll(oldActorsinThatMovie);
                }
            }
            //_context.SaveChanges();
            await _movieRepo.CommitAsync();
            await _ActorMovieRepo.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}