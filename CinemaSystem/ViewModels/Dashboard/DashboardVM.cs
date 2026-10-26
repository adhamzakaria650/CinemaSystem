namespace CinemaSystem.ViewModels.Dashboard
{
    public class ChartItem
    {
        public string Label { get; set; } = "";
        public int Value { get; set; }
    }
    public class DashboardVM
    {
        public int TotalMovies { get; set; }
        public int TotalCinemas { get; set; }
        public int TotalActors { get; set; }
        public int TotalCategories { get; set; }

        public int ActiveMovies { get; set; }
        public int InactiveMovies { get; set; }

        public List<ChartItem> MoviesPerCategory { get; set; } = new();
        public List<ChartItem> MoviesPerCinema { get; set; } = new();
        public List<ChartItem> TopActors { get; set; } = new();
    }
}
