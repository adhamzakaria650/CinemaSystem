using CinemaSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CinemaSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
        }
        public DbSet<Category> categories { get; set; }
        public DbSet<Cinema> cinemas { get; set; }
        public DbSet<Movie> movies { get; set; }
        public DbSet<Actor> actors { get; set; }
        public DbSet<MovieSubImage> movie_subimages { get; set; }
        public DbSet<ActorMovie> actor_movies { get; set; }
        public DbSet<ApplicationUserOTP> applicationUserOTPs { get; set; }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //    optionsBuilder.UseSqlServer("Data Source=.;initial catalog=CinemaSystem;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;");
        //}
    }
}
