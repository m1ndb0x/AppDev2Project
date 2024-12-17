using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using AppDev2Project.Models;
using Microsoft.AspNetCore.Identity;
using Azure.Storage.Blobs;
using AppDev2Project.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using AppDev2Project.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Register BlobStorageService as a singleton
builder.Services.AddSingleton<BlobStorageService>();

// Register DbContext with connection string stored in appsettings.json
builder.Services.AddDbContext<ExaminaDatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<ExaminaDatabaseContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Add a service to store the database connection status
builder.Services.AddSingleton<DatabaseConnectionStatus>();

// Add Azure Blob service for accessing Blob storage
builder.Services.AddSingleton(x => new BlobServiceClient(
    builder.Configuration["BlobStorageConnectionString"]));

// Update the authentication configuration
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";  // Verify this path
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Test database connection
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ExaminaDatabaseContext>();
    var dbStatus = scope.ServiceProvider.GetRequiredService<DatabaseConnectionStatus>();
    try
    {
        dbContext.Database.CanConnect();
        dbStatus.IsConnected = true;
        Console.WriteLine("Database connection successful.");
    }
    catch (Exception ex)
    {
        dbStatus.IsConnected = false;
        Console.WriteLine($"Database connection failed: {ex.Message}");
    }
}

// Add after builder.Services configuration
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var roles = new[] { "Teacher", "Student" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<int>(role));
        }
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Add this line before app.MapControllers()
app.UseUserActivity();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
