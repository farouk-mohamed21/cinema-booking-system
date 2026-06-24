namespace CinemaBooking2.ViewModels;

using System.ComponentModel.DataAnnotations;

public class RegisterVM
{
    [Required] public string FirstName { get; set; } = string.Empty;
    [Required] public string LastName { get; set; } = string.Empty;
    [Required][EmailAddress] public string Email { get; set; } = string.Empty;
    [Required]
    [MinLength(6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginVM
{
    [Required][EmailAddress] public string Email { get; set; } = string.Empty;
    [Required][DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}

public class ChangePasswordVM
{
    [Required][DataType(DataType.Password)] public string CurrentPassword { get; set; } = string.Empty;
    [Required][MinLength(6)][DataType(DataType.Password)] public string NewPassword { get; set; } = string.Empty;
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)] public string ConfirmPassword { get; set; } = string.Empty;
}

public class BookingVM
{
    public int ShowtimeId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public string HallName { get; set; } = string.Empty;
    public string CinemaName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
    [Required]
    [Range(1, 20, ErrorMessage = "You can book between 1 and 20 seats.")]
    public int SeatsCount { get; set; } = 1;
}

public class MovieVM
{
    public int Id { get; set; }
    [Required][MaxLength(200)] public string Title { get; set; } = string.Empty;
    [Required] public string Description { get; set; } = string.Empty;
    [Range(1, 500)] public int Duration { get; set; }
    public int CategoryId { get; set; }
    public string? ExistingPoster { get; set; }
    public IFormFile? Poster { get; set; }
}

public class ShowtimeVM
{
    public int Id { get; set; }
    [Required] public int MovieId { get; set; }
    [Required] public int HallId { get; set; }
    [Required] public DateTime StartTime { get; set; }
    [Range(0.01, 10000)] public decimal Price { get; set; }
}

public class CinemaVM
{
    public int Id { get; set; }
    [Required][MaxLength(100)] public string Name { get; set; } = string.Empty;
    [Required][MaxLength(200)] public string Location { get; set; } = string.Empty;
}

public class HallVM
{
    public int Id { get; set; }
    [Required][MaxLength(50)] public string Name { get; set; } = string.Empty;
    [Range(1, 1000)] public int Capacity { get; set; }
    [Required] public int CinemaId { get; set; }
}

public class CategoryVM
{
    public int Id { get; set; }
    [Required][MaxLength(50)] public string Name { get; set; } = string.Empty;
}