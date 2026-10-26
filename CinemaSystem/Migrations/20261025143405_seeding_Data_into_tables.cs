using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaSystem.Migrations
{
    /// <inheritdoc />
    public partial class seeding_Data_into_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "INSERT INTO categories (Name, Description, Status) VALUES ('Action', 'This is An action Movie', 1); " +
                "INSERT INTO cinemas (Name, Description, Status, Image) VALUES ('Misr', 'this is cinema misr', 0, 'cinema.png'); " +
                "INSERT INTO movies (Name, Description, Status, Price, Time, MainImg, categoryId, cinemaId) VALUES ('Fast And Furios', 'An Action Movie', 1, 100.50, '2026-10-25 17:43:00', 'movie.png', 1, 1); " +
                "INSERT INTO actors (Name, Description, Status, Image) VALUES ('Mohamed Ramadan', 'An Egyptian Actor', 0, 'actor.png'); " +
                "INSERT INTO movie_subimages (MovieId, Image) VALUES (1, 'Movie1Subimage.png'); " +
                "INSERT INTO actor_movies (ActorID, MovieID) VALUES (1, 1);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM actor_movies WHERE ActorID = 1 AND MovieID = 1; " +
                "DELETE FROM movie_subimages WHERE MovieId = 1 AND Image = 'Movie1Subimage.png'; " +
                "DELETE FROM actors WHERE Name = 'Mohamed Ramadan' AND Description = 'An Egyptian Actor'; " +
                "DELETE FROM movies WHERE Name = 'Fast And Furios' AND Description = 'An Action Movie'; " +
                "DELETE FROM cinemas WHERE Name = 'Misr' AND Description = 'this is cinema misr'; " +
                "DELETE FROM categories WHERE Name = 'Action' AND Description = 'This is An action Movie';");
        }
    }
}
