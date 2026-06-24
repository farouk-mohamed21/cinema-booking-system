namespace CinemaBooking2.Controllers;

using Microsoft.AspNetCore.Mvc;
using CinemaBooking2.Services;

public class HomeController : Controller
{
    private readonly ICinemaService _service;
    public HomeController(ICinemaService service) => _service = service;

    public async Task<IActionResult> Index()
    {
        var movies = await _service.GetMoviesAsync();
        return View(movies);
    }

    public async Task<IActionResult> Movies()
    {
        var movies = await _service.GetMoviesAsync();
        return View(movies);
    }

    public async Task<IActionResult> MovieDetails(int id)
    {
        var movie = await _service.GetMovieAsync(id);
        if (movie == null) return NotFound();
        return View(movie);
    }

    public async Task<IActionResult> Cinemas()
    {
        var cinemas = await _service.GetCinemasAsync();
        return View(cinemas);
    }

    public IActionResult Error(int? statusCode)
    {
        if (statusCode == 404) return View("Error404");
        return View("Error500");
    }
}