using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppDev2Project.Models;
using AppDev2Project.Services;

namespace AppDev2Project.Controllers
{
    public class PfpController : Controller
    {
        private readonly ExaminaDatabaseContext _dbContext;
        private readonly BlobStorageService _blobStorageService;
        private const string ContainerName = "examina"; // Blob container name

        public PfpController(ExaminaDatabaseContext dbContext, BlobStorageService blobStorageService)
        {
            _dbContext = dbContext;
            _blobStorageService = blobStorageService;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateDefaultPfp(string userName)
        {
            var nameParts = userName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string initials;

            if (nameParts.Length >= 2)
            {
                // Take first letter of first name and first letter of last name
                initials = $"{nameParts[0][0]}{nameParts[nameParts.Length - 1][0]}".ToUpper();
            }
            else
            {
                // If only one name, take first letter or first two letters
                initials = (nameParts[0].Length >= 2)
                    ? nameParts[0].Substring(0, 2).ToUpper()
                    : nameParts[0][0].ToString().ToUpper();
            }

            var avatarUrl = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(initials)}&background=random&color=fff&size=256";

            return Json(new { url = avatarUrl });
        }

        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a valid file.");
                return BadRequest("Invalid file.");
            }

            //get  the logged-in users email
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            //find user in db
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return Unauthorized();

            //create unique file name (ID + file extension)
            var fileName = $"{user.Id}{Path.GetExtension(file.FileName)}";

            // upload file to blob storage
            using (var stream = file.OpenReadStream())
            {
                await _blobStorageService.UploadFileAsync(ContainerName, fileName, stream);
            }

            //update user profile pfp URL in the DB
            user.ProfilePictureUrl = _blobStorageService.GetBlobUrl(ContainerName, fileName);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Profile picture updated successfully.", url = user.ProfilePictureUrl });
        }

        [HttpGet]
        public async Task<IActionResult> GetProfilePicture()
        {
            //get user's email
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            // Find user in db
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return Unauthorized();

            // Return their pfp URL or generate initials default
            var profilePictureUrl = string.IsNullOrEmpty(user.ProfilePictureUrl)
                ? GenerateDefaultPfpUrl(user.Name)
                : user.ProfilePictureUrl;

            return Json(new { url = profilePictureUrl });
        }

        // Helper method to generate default profile picture URL
        private string GenerateDefaultPfpUrl(string userName)
        {
            var initials = string.Join("", userName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries) // Remove empty entries
                .Where(w => !string.IsNullOrWhiteSpace(w))         // Filter empty or whitespace strings
                .Select(w => w[0]))                               // Safely get the first character
                .ToUpper();

            return $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(initials)}&background=random&color=fff&size=256";
        }
    }
}
