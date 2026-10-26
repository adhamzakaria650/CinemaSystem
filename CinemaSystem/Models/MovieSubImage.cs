using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaSystem.Models
{
    [PrimaryKey(nameof(MovieId),nameof(Image))]
    public class MovieSubImage
    {
        public int MovieId { get; set; }
        [ForeignKey(nameof(MovieId))]
        public Movie movie { get; set; }
        public string Image { get; set; }
    }
}
