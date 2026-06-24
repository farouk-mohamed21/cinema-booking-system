using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CinemaBooking2.Data;
using CinemaBooking2.Models;
using CinemaBooking2.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Database ─────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Identity ──────────────────────────────────────────────────
builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ── Cookie ────────────────────────────────────────────────────
builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.LoginPath = "/Account/Login";
    opt.AccessDeniedPath = "/Account/AccessDenied";
});

// ── DI ────────────────────────────────────────────────────────
builder.Services.AddScoped<ICinemaService, CinemaService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ── Seed Roles + First Admin ──────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var users = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    await db.Database.MigrateAsync();

    foreach (var role in new[] { "Admin", "Customer" })
        if (!await roles.RoleExistsAsync(role))
            await roles.CreateAsync(new IdentityRole(role));

    const string adminEmail = "admin@cinema.com";
    if (await users.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new AppUser
        {
            FirstName = "Super",
            LastName = "Admin",
            Email = adminEmail,
            UserName = adminEmail
        };
        await users.CreateAsync(admin, "Admin@123");
        await users.AddToRoleAsync(admin, "Admin");
        Console.WriteLine("✓ Admin created: admin@cinema.com / Admin@123");
    }
}

app.Run();