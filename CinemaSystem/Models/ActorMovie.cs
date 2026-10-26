using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaSystem.Models
{
    [PrimaryKey(nameof(ActorID),nameof(MovieID))]
    public class ActorMovie
    {
        public int ActorID { get; set; }
        [ForeignKey(nameof(ActorID))]
        public Actor actor { get; set; }
        public int MovieID { get; set; }
        [ForeignKey(nameof(MovieID))]
        public Movie movie { get; set; }
    }
}
