using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using AppDev2Project.Models;
using Microsoft.AspNetCore.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using AzureKeyVaultConfigurationOptions = Azure.Extensions.AspNetCore.Configuration.Secrets.AzureKeyVaultConfigurationOptions;

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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles(); // Serves static files (CSS, JS, etc.)

app.UseRouting();

// teacher controller
app.MapControllerRoute(
    name: "teacher",
    pattern: "{controller=Teacher}/{action=Index}/{id?}");

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
