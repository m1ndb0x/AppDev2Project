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
                Email = user.Email
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


        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(SettingsViewModel model)
        {
            Console.WriteLine($"Uploaded file details: Name={model.ProfilePictureFile?.FileName}, Size={model.ProfilePictureFile?.Length}");

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var userEmail = User.Identity?.Name;
            Console.WriteLine($"User email: {userEmail}");
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            Console.WriteLine($"User found: {user?.Id}");
            if (user == null)
                return Unauthorized();

            try
            {
                if (_blobStorageService == null)
                {
                    Console.WriteLine("BlobStorageService is null.");
                    throw new InvalidOperationException("BlobStorageService is not initialized.");
                }

                // Handle profile picture upload
                if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
                {
                    if (model.ProfilePictureFile.ContentType != "image/jpeg" && model.ProfilePictureFile.ContentType != "image/png")
                    {
                        ModelState.AddModelError("ProfilePictureFile", "Only JPEG or PNG files are allowed.");
                        return View("Index", model);
                    }

                    if (model.ProfilePictureFile.Length > 5 * 1024 * 1024) // 5 MB limit
                    {
                        ModelState.AddModelError("ProfilePictureFile", "File size must not exceed 5 MB.");
                        return View("Index", model);
                    }

                    var fileName = $"{user.Id}{Path.GetExtension(model.ProfilePictureFile.FileName)}";

                    using (var stream = model.ProfilePictureFile.OpenReadStream())
                    {
                        Console.WriteLine($"Attempting to upload file: {fileName} to Azure Blob Storage.");
                        await _blobStorageService.UploadFileAsync("examina", fileName, stream);
                        Console.WriteLine($"Successfully uploaded file: {fileName} to Azure Blob Storage.");
                    }

                    user.ProfilePictureUrl = _blobStorageService.GetBlobUrl("examina", fileName);
                    Console.WriteLine($"Generated ProfilePictureUrl: {user.ProfilePictureUrl}");
                    Console.WriteLine($"ProfilePictureUrl updated in memory: {user.ProfilePictureUrl}");
                }

                // Set initials-based URL as fallback if no picture uploaded
                if (string.IsNullOrEmpty(user.ProfilePictureUrl) && model.ProfilePictureFile == null)
                {
                    user.ProfilePictureUrl = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(user.Name)}&background=random&color=fff";
                    Console.WriteLine($"Fallback ProfilePictureUrl set: {user.ProfilePictureUrl}");
                }

                // Save changes
                await _dbContext.SaveChangesAsync();
                var updatedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
                Console.WriteLine($"Database ProfilePictureUrl after SaveChangesAsync: {updatedUser?.ProfilePictureUrl}");

                Console.WriteLine($"Saving ProfilePictureUrl to DB: {user.ProfilePictureUrl}");

                TempData["SuccessMessage"] = "Profile picture updated successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("ProfilePictureFile", "Failed to upload profile picture. Please try again.");
                return View("Index", model);
            }

            return RedirectToAction("Index");
        }

    }
}

// [HttpPost]
// public async Task<IActionResult> UpdateName(string name)
// {
//     if (string.IsNullOrWhiteSpace(name))
//     {
//         TempData["ErrorMessage"] = "Name cannot be empty.";
//         return RedirectToAction(nameof(Index));
//     }

//     var user = await _userManager.GetUserAsync(User);
//     if (user == null) return NotFound();

//     user.Name = name;
//     var result = await _userManager.UpdateAsync(user);

//     if (result.Succeeded)
//     {
//         TempData["SuccessMessage"] = "Your name has been updated.";
//     }
//     else
//     {
//         TempData["ErrorMessage"] = "Failed to update your name.";
//     }

//     return RedirectToAction(nameof(Index));
// }

// [HttpPost]
// public async Task<IActionResult> UpdateEmail(string email)
// {
//     if (string.IsNullOrWhiteSpace(email))
//     {
//         TempData["ErrorMessage"] = "Email cannot be empty.";
//         return RedirectToAction(nameof(Index));
//     }

//     var user = await _userManager.GetUserAsync(User);
//     if (user == null) return NotFound();

//     user.Email = email;
//     user.UserName = email; // Update UserName if using email as the username.
//     var result = await _userManager.UpdateAsync(user);

//     if (result.Succeeded)
//     {
//         TempData["SuccessMessage"] = "Your email has been updated.";
//     }
//     else
//     {
//         TempData["ErrorMessage"] = "Failed to update your email.";
//     }

//     return RedirectToAction(nameof(Index));
// }

// [HttpPost]
// public async Task<IActionResult> UpdatePassword(string currentPassword, string newPassword)
// {
//     if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
//     {
//         TempData["ErrorMessage"] = "Password fields cannot be empty.";
//         return RedirectToAction(nameof(Index));
//     }

//     var user = await _userManager.GetUserAsync(User);
//     if (user == null) return NotFound();

//     var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

//     if (result.Succeeded)
//     {
//         TempData["SuccessMessage"] = "Your password has been updated.";
//     }
//     else
//     {
//         foreach (var error in result.Errors)
//         {
//             TempData["ErrorMessage"] += error.Description + " ";
//         }
//     }

//     return RedirectToAction(nameof(Index));
// }
//}
//}

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
