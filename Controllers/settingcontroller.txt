using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AppDev2Project.Models;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AppDev2Project.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public SettingsController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
