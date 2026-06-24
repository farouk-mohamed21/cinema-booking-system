namespace CinemaBooking2.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CinemaBooking2.Models;
using CinemaBooking2.Services;
using CinemaBooking2.ViewModels;

[Authorize]
public class BookingController : Controller
{
    private readonly ICinemaService _service;
    private readonly UserManager<AppUser> _users;

    public BookingController(ICinemaService service, UserManager<AppUser> users)
    {
        _service = service;
        _users = users;
    }

    [HttpGet]
    public async Task<IActionResult> Book(int showtimeId)
    {
        var showtime = await _service.GetShowtimeAsync(showtimeId);
        if (showtime == null) return NotFound();

        if (showtime.HasStarted)
        {
            TempData["Error"] = "This showtime has already started.";
            return RedirectToAction("Movies", "Home");
        }

        var vm = new BookingVM
        {
            ShowtimeId = showtime.Id,
            MovieTitle = showtime.Movie.Title,
            HallName = showtime.Hall.Name,
            CinemaName = showtime.Hall.Cinema.Name,
            StartTime = showtime.StartTime,
            Price = showtime.Price,
            AvailableSeats = showtime.AvailableSeats
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Book(BookingVM vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var userId = _users.GetUserId(User)!;
        var (success, message) = await _service.BookAsync(userId, vm.ShowtimeId, vm.SeatsCount);

        if (success)
        {
            TempData["Success"] = message;
            return RedirectToAction("MyBookings");
        }

        TempData["Error"] = message;
        return View(vm);
    }

    public async Task<IActionResult> MyBookings()
    {
        var userId = _users.GetUserId(User)!;
        var bookings = await _service.GetUserBookingsAsync(userId);
        return View(bookings);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int bookingId)
    {
        var userId = _users.GetUserId(User)!;
        var (success, message) = await _service.CancelAsync(userId, bookingId);

        if (success) TempData["Success"] = message;
        else TempData["Error"] = message;

        return RedirectToAction("MyBookings");
    }
}