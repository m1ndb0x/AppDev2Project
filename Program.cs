using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using AppDev2Project.Models;


var builder = WebApplication.CreateBuilder(args);



// Azure Key Vault for securing private info
builder.Configuration.AddAzureKeyVault(
    new Uri("https://examina-keyvault.vault.azure.net/"),
    new DefaultAzureCredential()
);



// Register DbContext with connection string stored in appsettings.json
builder.Services.AddDbContext<ExaminaDatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration["DefaultConnection"]));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews(); 


var app = builder.Build();

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

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages();

app.Run();
