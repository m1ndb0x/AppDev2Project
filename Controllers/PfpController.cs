// using Microsoft.AspNetCore.Mvc;
// using AppDev2Project.Services;
// using AppDev2Project.Models;
// using System.IO;
// using System.Threading.Tasks;

// namespace AppDev2Project.Controllers
// {
//     public class PfpController : Controller
//     {
//         private readonly BlobStorageService _blobStorageService;
//         private readonly ExaminaDatabaseContext _dbContext; // DbContext to manage User table
//         private const string ContainerName = "examina"; // blob container name

//         public PfpController(BlobStorageService blobStorageService, ExaminaDatabaseContext dbContext)
//         {
//             _blobStorageService = blobStorageService;
//             _dbContext = dbContext;
//         }

//         // Action to render the profile picture upload form
//         [HttpGet]
//         public IActionResult Upload()
//         {
//             return View();
//         }

//         // Action to handle profile picture uploads
//         [HttpPost]
//         public async Task<IActionResult> Upload(IFormFile file)
//         {
//             if (file == null || file.Length == 0)
//             {
//                 ModelState.AddModelError("File", "Please select a valid file.");
//                 return View();
//             }

//             var userEmail = User.Identity.Name; // Assuming Identity.Name stores the email...
//             var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
//             if (user == null) return Unauthorized();
//             var userId = user.Id;
//             var user = await _dbContext.Users.FindAsync(userId);

//             if (user == null)
//             {
//                 return NotFound("User not found.");
//             }

//             // Create a unique file name (e.g., userId + file extension)
//             var fileName = $"{userId}{Path.GetExtension(file.FileName)}";

//             using (var stream = file.OpenReadStream())
//             {
//                 await _blobStorageService.UploadFileAsync(ContainerName, fileName, stream);
//             }

//             // Update user's Pfp URL
//             user.PfpUrl = $"{_blobStorageService.GetBlobUrl(ContainerName, fileName)}";
//             await _dbContext.SaveChangesAsync();

// if (User.IsInRole("Student"))
// {
//     return RedirectToAction("Index", "StudentDashboard");
// }
// else if (User.IsInRole("Teacher"))
// {
//     return RedirectToAction("Index", "TeacherDashboard");
// }
//         }

//         // Action to get a user's profile picture
//         [HttpGet]
//         public async Task<IActionResult> GetPfp(string userId)
//         {
//             var user = await _dbContext.Users.FindAsync(userId);

//             if (user == null || string.IsNullOrEmpty(user.PfpUrl))
//             {
//                 // Generate default initials-based picture
//                 return File(GenerateInitialsImage(user?.Name ?? "User"), "image/png");
//             }

//             return Redirect(user.PfpUrl); // Redirect to blob URL
//         }

//         // Utility method to generate initials-based image
//         private byte[] GenerateInitialsImage(string name)
//         {
//             // Implement initials-based image generation logic (e.g., using System.Drawing or a library)
//             // Placeholder for now:
//             return System.Text.Encoding.UTF8.GetBytes("Initials Placeholder");
//         }
//     }
// }
