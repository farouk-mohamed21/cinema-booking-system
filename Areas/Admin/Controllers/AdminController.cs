namespace CinemaBooking2.Areas.Admin.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaBooking2.Data;
using CinemaBooking2.Models;
using CinemaBooking2.Services;
using CinemaBooking2.ViewModels;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _db;
    private readonly ICinemaService _service;
    private readonly IWebHostEnvironment _env;

    public AdminController(AppDbContext db, ICinemaService service, IWebHostEnvironment env)
    {
        _db = db;
        _service = service;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Movies = await _db.Movies.CountAsync();
        ViewBag.Cinemas = await _db.Cinemas.CountAsync();
        ViewBag.Bookings = await _db.Bookings.CountAsync();
        ViewBag.Showtimes = await _db.Showtimes.CountAsync();
        return View();
    }

    public async Task<IActionResult> Bookings()
        => View(await _service.GetAllBookingsAsync());

    // ── Movies ────────────────────────────────────────────────
    public async Task<IActionResult> Movies()
        => View(await _db.Movies.Include(m => m.Category).ToListAsync());

    [HttpGet]
    public async Task<IActionResult> CreateMovie()
    {
        ViewBag.Categories = new SelectList(await _db.Categories.ToListAsync(), "Id", "Name");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateMovie(MovieVM vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(await _db.Categories.ToListAsync(), "Id", "Name");
            return View(vm);
        }
        var movie = new Movie
        {
            Title = vm.Title,
            Description = vm.Description,
            Duration = vm.Duration,
            CategoryId = vm.CategoryId,
            PosterPath = await SavePosterAsync(vm.Poster)
        };
        _db.Movies.Add(movie);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Movie created successfully.";
        return RedirectToAction("Movies");
    }

    [HttpGet]
    public async Task<IActionResult> EditMovie(int id)
    {
        var movie = await _db.Movies.FindAsync(id);
        if (movie == null) return NotFound();
        ViewBag.Categories = new SelectList(await _db.Categories.ToListAsync(), "Id", "Name");
        return View(new MovieVM
        {
            Id = movie.Id,
            Title = movie.Title,
            Description = movie.Description,
            Duration = movie.Duration,
            CategoryId = movie.CategoryId,
            ExistingPoster = movie.PosterPath
        });
    }

    [HttpPost]
    public async Task<IActionResult> EditMovie(MovieVM vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(await _db.Categories.ToListAsync(), "Id", "Name");
            return View(vm);
        }
        var movie = await _db.Movies.FindAsync(vm.Id);
        if (movie == null) return NotFound();
        movie.Title = vm.Title; movie.Description = vm.Description;
        movie.Duration = vm.Duration; movie.CategoryId = vm.CategoryId;
        if (vm.Poster != null) movie.PosterPath = await SavePosterAsync(vm.Poster);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Movie updated successfully.";
        return RedirectToAction("Movies");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        var movie = await _db.Movies.FindAsync(id);
        if (movie != null) { _db.Movies.Remove(movie); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Movie deleted.";
        return RedirectToAction("Movies");
    }

    // ── Categories ────────────────────────────────────────────
    public async Task<IActionResult> Categories()
        => View(await _db.Categories.Include(c => c.Movies).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CategoryVM vm)
    {
        if (!ModelState.IsValid) { TempData["Error"] = "Invalid data."; return RedirectToAction("Categories"); }
        _db.Categories.Add(new Category { Name = vm.Name });
        await _db.SaveChangesAsync();
        TempData["Success"] = "Category created.";
        return RedirectToAction("Categories");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var cat = await _db.Categories.FindAsync(id);
        if (cat != null) { _db.Categories.Remove(cat); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Category deleted.";
        return RedirectToAction("Categories");
    }

    // ── Cinemas ───────────────────────────────────────────────
    public async Task<IActionResult> Cinemas()
        => View(await _db.Cinemas.Include(c => c.Halls).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> CreateCinema(CinemaVM vm)
    {
        if (!ModelState.IsValid) { TempData["Error"] = "Invalid data."; return RedirectToAction("Cinemas"); }
        _db.Cinemas.Add(new Cinema { Name = vm.Name, Location = vm.Location });
        await _db.SaveChangesAsync();
        TempData["Success"] = "Cinema created.";
        return RedirectToAction("Cinemas");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCinema(int id)
    {
        var cinema = await _db.Cinemas.FindAsync(id);
        if (cinema != null) { _db.Cinemas.Remove(cinema); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Cinema deleted.";
        return RedirectToAction("Cinemas");
    }

    [HttpPost]
    public async Task<IActionResult> CreateHall(HallVM vm)
    {
        if (!ModelState.IsValid) { TempData["Error"] = "Invalid data."; return RedirectToAction("Cinemas"); }
        _db.Halls.Add(new Hall { Name = vm.Name, Capacity = vm.Capacity, CinemaId = vm.CinemaId });
        await _db.SaveChangesAsync();
        TempData["Success"] = "Hall created.";
        return RedirectToAction("Cinemas");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteHall(int id)
    {
        var hall = await _db.Halls.FindAsync(id);
        if (hall != null) { _db.Halls.Remove(hall); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Hall deleted.";
        return RedirectToAction("Cinemas");
    }

    // ── Showtimes ─────────────────────────────────────────────
    public async Task<IActionResult> Showtimes()
        => View(await _db.Showtimes
            .Include(s => s.Movie)
            .Include(s => s.Hall).ThenInclude(h => h.Cinema)
            .ToListAsync());

    [HttpGet]
    public async Task<IActionResult> CreateShowtime()
    {
        ViewBag.Movies = new SelectList(await _db.Movies.ToListAsync(), "Id", "Title");
        ViewBag.Halls = new SelectList(await _db.Halls.Include(h => h.Cinema)
            .Select(h => new { h.Id, Name = h.Cinema.Name + " - " + h.Name })
            .ToListAsync(), "Id", "Name");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateShowtime(ShowtimeVM vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Movies = new SelectList(await _db.Movies.ToListAsync(), "Id", "Title");
            ViewBag.Halls = new SelectList(await _db.Halls.Include(h => h.Cinema)
                .Select(h => new { h.Id, Name = h.Cinema.Name + " - " + h.Name })
                .ToListAsync(), "Id", "Name");
            return View(vm);
        }
        var hall = await _db.Halls.FindAsync(vm.HallId);
        _db.Showtimes.Add(new Showtime
        {
            MovieId = vm.MovieId,
            HallId = vm.HallId,
            StartTime = vm.StartTime,
            Price = vm.Price,
            AvailableSeats = hall!.Capacity
        });
        await _db.SaveChangesAsync();
        TempData["Success"] = "Showtime created.";
        return RedirectToAction("Showtimes");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteShowtime(int id)
    {
        var s = await _db.Showtimes.FindAsync(id);
        if (s != null) { _db.Showtimes.Remove(s); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Showtime deleted.";
        return RedirectToAction("Showtimes");
    }

    private async Task<string?> SavePosterAsync(IFormFile? file)
    {
        if (file == null) return null;
        var folder = Path.Combine(_env.WebRootPath, "posters");
        Directory.CreateDirectory(folder);
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var path = Path.Combine(folder, fileName);
        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);
        return "/posters/" + fileName;
    }
}