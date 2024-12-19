using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AppDev2Project.Models;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using AppDev2Project.Services;

namespace AppDev2Project.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly ExaminaDatabaseContext _dbContext;
        private readonly BlobStorageService _blobStorageService;

        public SettingsController(UserManager<User> userManager,
                                    SignInManager<User> signInManager,
                                     ExaminaDatabaseContext dbContext,
                                     BlobStorageService blobStorageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _blobStorageService = blobStorageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new SettingsViewModel
            {
                Name = user.Name,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateName(string name)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (string.IsNullOrEmpty(name))
            {
                TempData["ErrorMessage"] = "Name is required.";
                return RedirectToAction(nameof(Index));
            }

            user.Name = name;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Name updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update name.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmail(string email)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Email is required.";
                return RedirectToAction(nameof(Index));
            }

            var emailExists = await _userManager.FindByEmailAsync(email);
            if (emailExists != null && emailExists.Id != user.Id)
            {
                TempData["ErrorMessage"] = "This email is already in use.";
                return RedirectToAction(nameof(Index));
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, email);
            var result = await _userManager.ChangeEmailAsync(user, email, token);
            if (result.Succeeded)
            {
                await _userManager.SetUserNameAsync(user, email);
                TempData["SuccessMessage"] = "Email updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update email.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "New password and confirmation password do not match.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Password updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update password. Please check your current password.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Password is required to delete your account.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                TempData["ErrorMessage"] = "Incorrect password.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Failed to delete account. Please try again.";
                return RedirectToAction(nameof(Index));
            }

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(SettingsViewModel model)
        {
            if (model.ProfilePictureFile == null || model.ProfilePictureFile.Length == 0)
            {
                ModelState.AddModelError("ProfilePictureFile", "Please select a file.");
                return View("Index", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            try
            {
                // Validate file
                if (!IsValidImage(model.ProfilePictureFile))
                {
                    ModelState.AddModelError("ProfilePictureFile", "Only JPEG or PNG files up to 5MB are allowed.");
                    return View("Index", model);
                }

                // Create unique filename
                string fileName = $"profile-{user.Id}-{DateTime.UtcNow.Ticks}{Path.GetExtension(model.ProfilePictureFile.FileName)}";
                
                // Upload to Azure Blob Storage
                using (var stream = model.ProfilePictureFile.OpenReadStream())
                {
                    await _blobStorageService.UploadFileAsync("examinablob", fileName, stream);
                }

                // Update user profile with new image URL
                user.ProfilePictureUrl = _blobStorageService.GetBlobUrl("examinablob", fileName);

                await _userManager.UpdateAsync(user);

                TempData["SuccessMessage"] = "Profile picture updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to upload profile picture. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveProfilePicture()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            try
            {
                // Generate initials from name
                var nameParts = user.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string initials;
                
                if (nameParts.Length >= 2)
                {
                    // Take first letter of first name and first letter of last name
                    initials = $"{nameParts[0][0]}{nameParts[^1][0]}".ToUpper();
                }
                else
                {
                    // If only one name, take first letter
                    initials = nameParts[0][0].ToString().ToUpper();
                }

                // Set default avatar URL with initials
                user.ProfilePictureUrl = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(initials)}&background=random&color=fff&size=256";
                
                await _userManager.UpdateAsync(user);
                TempData["SuccessMessage"] = "Profile picture removed successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to remove profile picture. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool IsValidImage(IFormFile file)
        {
            if (file.Length > 5 * 1024 * 1024) // 5MB limit
                return false;

            string[] allowedTypes = { "image/jpeg", "image/png" };
            return allowedTypes.Contains(file.ContentType.ToLower());
        }
    }
}
