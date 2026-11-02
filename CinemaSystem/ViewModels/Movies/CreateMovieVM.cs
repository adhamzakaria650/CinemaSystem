using CinemaSystem.CustomValidations;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace CinemaSystem.ViewModels.Movies
{
    public class CreateMovieVM
    {
        [MinLength(3)]
        [MaxLength(20)]
        public string Name { get; set; }
        [OneImageSizeAttribute]
        public IFormFile MainImage { get; set; }
        [ManyImageSizeAttribute]
        public List<IFormFile> SubImages { get; set; }
        public int CategoryId { get; set; }
        public int CinemaId { get; set; }
        public DateTime Time { get; set;  }
        public List<int> ActorId { get; set; }
        [Range(20,1000)]
        public double Price {  get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }

    }
}
