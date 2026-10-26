namespace CinemaSystem.ViewModels.Movies
{
    public class UpdateMovieVM
    {
        public int Id { get; set; }
        public string Name {  get; set; }
        public IFormFile MainImage { get; set; }
        public List<IFormFile> SubImages {  get; set; }
        public int CategoryId { get; set; }
        public int CinemaId { get; set;  }
        public List<int> ActorId {  get; set; }
        public double Price {  get; set; }
        public DateTime? Time { get; set; } = null;
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}
