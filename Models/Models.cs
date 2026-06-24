namespace CinemaBooking2.Models;

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class AppUser : IdentityUser
{
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

public class Category
{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}

public class Movie
{
    public int Id { get; set; }
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Range(1, 500)]
    public int Duration { get; set; }
    public string? PosterPath { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}

public class Cinema
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;
    public ICollection<Hall> Halls { get; set; } = new List<Hall>();
}

public class Hall
{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [Range(1, 1000)]
    public int Capacity { get; set; }
    public int CinemaId { get; set; }
    public Cinema Cinema { get; set; } = null!;
    public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}

public class Showtime
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    [Column(TypeName = "decimal(8,2)")]
    [Range(0.01, 10000)]
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
    public int MovieId { get; set; }
    public int HallId { get; set; }
    public Movie Movie { get; set; } = null!;
    public Hall Hall { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public bool HasStarted => StartTime <= DateTime.Now;
}

public class Booking
{
    public int Id { get; set; }
    public int SeatsCount { get; set; }
    [Column(TypeName = "decimal(8,2)")]
    public decimal TotalPrice { get; set; }
    public DateTime BookingDate { get; set; } = DateTime.Now;
    public string UserId { get; set; } = string.Empty;
    public int ShowtimeId { get; set; }
    public AppUser User { get; set; } = null!;
    public Showtime Showtime { get; set; } = null!;
    public bool CanCancel => !Showtime.HasStarted;
}