using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using AppDev2Project.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

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

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapRazorPages();
app.Run();
