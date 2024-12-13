using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using AppDev2Project.Models;
using Microsoft.AspNetCore.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets; // Add this
using Azure.Security.KeyVault.Secrets; // Add this
using Microsoft.Extensions.Configuration.AzureKeyVault;
using AzureKeyVaultConfigurationOptions = Azure.Extensions.AspNetCore.Configuration.Secrets.AzureKeyVaultConfigurationOptions; // Add this

var builder = WebApplication.CreateBuilder(args);

// Azure Key Vault for securing private info
builder.Configuration.AddAzureKeyVault(
    new Uri("https://examina-keyvault2.vault.azure.net/"),
    new DefaultAzureCredential(),
    new AzureKeyVaultConfigurationOptions
    {
        ReloadInterval = TimeSpan.FromMinutes(5),
        Manager = new KeyVaultSecretManager()
    }
);

// // Register DbContext with connection string stored in appsettings.json
// builder.Services.AddDbContext<ExaminaDatabaseContext>(options =>
//     options.UseSqlServer(builder.Configuration["DefaultConnection"]));

// Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ExaminaDatabaseContext>()
    .AddDefaultTokenProviders();
    
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews(); 

var app = builder.Build();

// Test database connection
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ExaminaDatabaseContext>();
    try
    {
        dbContext.Database.CanConnect();
        Console.WriteLine("Database connection successful.");
    }
    catch (Exception ex)
    {
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

app.MapStaticAssets();
app.MapRazorPages();

app.Run();
