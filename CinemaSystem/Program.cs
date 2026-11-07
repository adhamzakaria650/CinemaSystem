using CinemaSystem.Data;
using CinemaSystem.Models;
using CinemaSystem.Repositories;
using CinemaSystem.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace CinemaSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllersWithViews();


            var Connection = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("Can't Connect With Your DataBase");


            builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(Connection)
                );
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;

            }).AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();


            builder.Services.AddScoped<IRepo<Actor>, Repo<Actor>>();
            builder.Services.AddScoped<IMovieActorsRepo, ActorMovieRepo>();
            builder.Services.AddScoped<IRepo<Category>, Repo<Category>>();
            builder.Services.AddScoped<IRepo<Cinema>, Repo<Cinema>>();
            builder.Services.AddScoped<IRepo<Movie>, Repo<Movie>>();
            builder.Services.AddScoped<IMovieSubImageRepo , MovieSubImageRepo>();
            builder.Services.AddTransient<IEmailSender,EmailSender>();
            builder.Services.AddScoped<IRepo<ApplicationUserOTP>, Repo<ApplicationUserOTP>>();


            



            var app = builder.Build();



            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{Area=Identity}/{controller=Account}/{action=Register}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
