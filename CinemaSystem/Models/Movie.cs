using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaSystem.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
        public double Price { get; set; }
        public DateTime Time { get; set; }
        public string MainImg { get; set; }
        public int categoryId { get; set; }
        [ForeignKey(nameof(categoryId))]
        public Category category { get; set; }
        public int cinemaId { get; set; }
        [ForeignKey(nameof(cinemaId))]
        public Cinema cinema { get; set; }
        public ICollection<ActorMovie> actorMovies { get; set; }

    }
}
