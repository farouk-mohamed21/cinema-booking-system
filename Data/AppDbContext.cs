namespace CinemaBooking2.Data;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CinemaBooking2.Models;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Cinema> Cinemas { get; set; }
    public DbSet<Hall> Halls { get; set; }
    public DbSet<Showtime> Showtimes { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);

        model.Entity<Movie>(e =>
        {
            e.HasOne(m => m.Category).WithMany(c => c.Movies)
             .HasForeignKey(m => m.CategoryId).OnDelete(DeleteBehavior.Restrict);
        });

        model.Entity<Hall>(e =>
        {
            e.HasOne(h => h.Cinema).WithMany(c => c.Halls)
             .HasForeignKey(h => h.CinemaId).OnDelete(DeleteBehavior.Cascade);
        });

        model.Entity<Showtime>(e =>
        {
            e.HasOne(s => s.Movie).WithMany(m => m.Showtimes)
             .HasForeignKey(s => s.MovieId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(s => s.Hall).WithMany(h => h.Showtimes)
             .HasForeignKey(s => s.HallId).OnDelete(DeleteBehavior.Restrict);
        });

        model.Entity<Booking>(e =>
        {
            e.HasOne(b => b.User).WithMany(u => u.Bookings)
             .HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(b => b.Showtime).WithMany(s => s.Bookings)
             .HasForeignKey(b => b.ShowtimeId).OnDelete(DeleteBehavior.Restrict);
        });

        // Seed Categories
        model.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Action" },
            new Category { Id = 2, Name = "Comedy" },
            new Category { Id = 3, Name = "Drama" },
            new Category { Id = 4, Name = "Horror" },
            new Category { Id = 5, Name = "Sci-Fi" },
            new Category { Id = 6, Name = "Romance" }
        );

        // Seed Cinemas
        model.Entity<Cinema>().HasData(
            new Cinema { Id = 1, Name = "Grand Cinema", Location = "Cairo" },
            new Cinema { Id = 2, Name = "Star Cinema", Location = "Alexandria" }
        );

        // Seed Halls
        model.Entity<Hall>().HasData(
            new Hall { Id = 1, Name = "Hall A", Capacity = 100, CinemaId = 1 },
            new Hall { Id = 2, Name = "Hall B", Capacity = 80, CinemaId = 1 },
            new Hall { Id = 3, Name = "Hall A", Capacity = 120, CinemaId = 2 }
        );

        // Seed Movies
        model.Entity<Movie>().HasData(
            new Movie { Id = 1, Title = "Inception", Description = "A mind-bending thriller.", Duration = 148, CategoryId = 5 },
            new Movie { Id = 2, Title = "The Dark Knight", Description = "Batman vs Joker.", Duration = 152, CategoryId = 1 },
            new Movie { Id = 3, Title = "Interstellar", Description = "Journey through space.", Duration = 169, CategoryId = 5 },
            new Movie { Id = 4, Title = "The Notebook", Description = "A timeless love story.", Duration = 123, CategoryId = 6 },
            new Movie { Id = 5, Title = "Get Out", Description = "A horror masterpiece.", Duration = 104, CategoryId = 4 }
        );

        // Seed Showtimes — fixed dates
        model.Entity<Showtime>().HasData(
            new Showtime { Id = 1, MovieId = 1, HallId = 1, StartTime = new DateTime(2025, 12, 1, 18, 0, 0), Price = 50, AvailableSeats = 100 },
            new Showtime { Id = 2, MovieId = 2, HallId = 2, StartTime = new DateTime(2025, 12, 1, 20, 0, 0), Price = 60, AvailableSeats = 80 },
            new Showtime { Id = 3, MovieId = 3, HallId = 3, StartTime = new DateTime(2025, 12, 2, 18, 0, 0), Price = 55, AvailableSeats = 120 },
            new Showtime { Id = 4, MovieId = 4, HallId = 1, StartTime = new DateTime(2025, 12, 2, 20, 0, 0), Price = 45, AvailableSeats = 100 },
            new Showtime { Id = 5, MovieId = 5, HallId = 2, StartTime = new DateTime(2025, 12, 3, 18, 0, 0), Price = 50, AvailableSeats = 80 }
        );
    }
}