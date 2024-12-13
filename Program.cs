using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using AppDev2Project;


var builder = WebApplication.CreateBuilder(args);

/*
// Azure Key Vault
builder.Configuration.AddAzureKeyVault(
    new Uri("https://your-keyvault-name.vault.azure.net/"), // Replace with key vault URL. TODO 
    new DefaultAzureCredential()
);
*/

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

app.UseRouting();

// teacher controller
app.MapControllerRoute(
    name: "teacher",
    pattern: "{controller=Teacher}/{action=Index}/{id?}");

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages();

app.Run();
