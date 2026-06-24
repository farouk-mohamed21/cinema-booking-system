namespace CinemaBooking2.Services;

using Microsoft.EntityFrameworkCore;
using CinemaBooking2.Data;
using CinemaBooking2.Models;

public interface ICinemaService
{
    Task<List<Movie>> GetMoviesAsync();
    Task<Movie?> GetMovieAsync(int id);
    Task<List<Cinema>> GetCinemasAsync();
    Task<Cinema?> GetCinemaAsync(int id);
    Task<Showtime?> GetShowtimeAsync(int id);
    Task<(bool success, string message)> BookAsync(string userId, int showtimeId, int seats);
    Task<(bool success, string message)> CancelAsync(string userId, int bookingId);
    Task<List<Booking>> GetUserBookingsAsync(string userId);
    Task<List<Booking>> GetAllBookingsAsync();
}

public class CinemaService : ICinemaService
{
    private readonly AppDbContext _db;
    public CinemaService(AppDbContext db) => _db = db;

    public async Task<List<Movie>> GetMoviesAsync() =>
        await _db.Movies.AsNoTracking()
            .Include(m => m.Category)
            .Include(m => m.Showtimes)
            .ToListAsync();

    public async Task<Movie?> GetMovieAsync(int id) =>
        await _db.Movies.AsNoTracking()
            .Include(m => m.Category)
            .Include(m => m.Showtimes)
                .ThenInclude(s => s.Hall)
                    .ThenInclude(h => h.Cinema)
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<List<Cinema>> GetCinemasAsync() =>
        await _db.Cinemas.AsNoTracking()
            .Include(c => c.Halls)
            .ToListAsync();

    public async Task<Cinema?> GetCinemaAsync(int id) =>
        await _db.Cinemas.AsNoTracking()
            .Include(c => c.Halls)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Showtime?> GetShowtimeAsync(int id) =>
        await _db.Showtimes
            .Include(s => s.Movie)
            .Include(s => s.Hall).ThenInclude(h => h.Cinema)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<(bool success, string message)> BookAsync(
        string userId, int showtimeId, int seats)
    {
        var showtime = await _db.Showtimes.FindAsync(showtimeId);
        if (showtime == null) return (false, "Showtime not found.");
        if (showtime.HasStarted) return (false, "This showtime has already started.");
        if (seats > showtime.AvailableSeats)
            return (false, $"Only {showtime.AvailableSeats} seats available.");
        if (seats <= 0) return (false, "Please select at least 1 seat.");

        var booking = new Booking
        {
            UserId = userId,
            ShowtimeId = showtimeId,
            SeatsCount = seats,
            TotalPrice = seats * showtime.Price,
            BookingDate = DateTime.Now
        };

        showtime.AvailableSeats -= seats;
        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();
        return (true, $"Booking confirmed! {seats} seat(s) for ${booking.TotalPrice:F2}");
    }

    public async Task<(bool success, string message)> CancelAsync(
        string userId, int bookingId)
    {
        var booking = await _db.Bookings
            .Include(b => b.Showtime)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

        if (booking == null) return (false, "Booking not found.");
        if (booking.Showtime.HasStarted)
            return (false, "Cannot cancel — showtime has already started.");

        booking.Showtime.AvailableSeats += booking.SeatsCount;
        _db.Bookings.Remove(booking);
        await _db.SaveChangesAsync();
        return (true, "Booking cancelled successfully.");
    }

    public async Task<List<Booking>> GetUserBookingsAsync(string userId) =>
        await _db.Bookings.AsNoTracking()
            .Include(b => b.Showtime).ThenInclude(s => s.Movie)
            .Include(b => b.Showtime).ThenInclude(s => s.Hall).ThenInclude(h => h.Cinema)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();

    public async Task<List<Booking>> GetAllBookingsAsync() =>
        await _db.Bookings.AsNoTracking()
            .Include(b => b.User)
            .Include(b => b.Showtime).ThenInclude(s => s.Movie)
            .Include(b => b.Showtime).ThenInclude(s => s.Hall).ThenInclude(h => h.Cinema)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
}