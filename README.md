# 🎬 Cinema Booking System

A .NET 10 ASP.NET Core MVC web application for booking cinema tickets online.

---

## 🚀 How to Run

### 1. Clone the repo
```bash
git clone https://github.com/farouk-mohamed21/cinema-booking-system.git
cd cinema-booking-system
```

### 2. Update connection string in `appsettings.json`
```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=CinemaBooking2;Trusted_Connection=True;TrustServerCertificate=True;"
```

### 3. Run migrations
```bash
dotnet ef database update --connection "Server=.\SQLEXPRESS;Database=CinemaBooking2;Trusted_Connection=True;TrustServerCertificate=True;"
```

### 4. Run the app
```bash
dotnet run
```

---

## 🔐 Default Admin Account
| Field | Value |
|---|---|
| Email | admin@cinema.com |
| Password | Admin@123 |

---

## 🖼 Movie Posters
Uploaded posters are stored in:
wwwroot/posters/

---

## ✅ Features
| Feature | Details |
|---|---|
| Authentication | Register, Login, Logout, Change Password |
| Roles | Admin and Customer |
| Public Pages | Home, Movies, Movie Details, Cinemas |
| Customer | Book tickets, View bookings, Cancel bookings |
| Admin | Manage Movies, Categories, Cinemas, Halls, Showtimes |
| Validation | Client-side and server-side |
| Live Price | Total price updates without page reload |
| Error Pages | Custom 404 and 500 pages |
| Responsive | Mobile-friendly with Bootstrap 5 |

---

## 📁 Project Structure
CinemaBooking2/

├── Areas/Admin/       # Admin area

├── Controllers/       # Home, Account, Booking

├── Data/              # AppDbContext

├── Migrations/        # EF Core migrations

├── Models/            # Domain entities

├── Services/          # Business logic

├── ViewModels/        # ViewModels

├── Views/             # Razor views

└── wwwroot/posters/   # Uploaded movie posters

