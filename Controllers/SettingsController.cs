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
        public async Task<IActionResult> UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["ErrorMessage"] = "Name cannot be empty.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.Name = name;
            var result = await _userManager.UpdateAsync(user);

            TempData["SuccessMessage"] = result.Succeeded ? "Your name has been updated." : "Failed to update your name.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["ErrorMessage"] = "Email cannot be empty.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.Email = email;
            user.UserName = email; // Optional if using email as username
            var result = await _userManager.UpdateAsync(user);

            TempData["SuccessMessage"] = result.Succeeded ? "Your email has been updated." : "Failed to update your email.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                TempData["ErrorMessage"] = "Password fields cannot be empty.";
                return RedirectToAction(nameof(Index));
            }

            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "The new password and confirmation password do not match.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            TempData["SuccessMessage"] = result.Succeeded ? "Your password has been updated." : "Failed to update your password.";
            return RedirectToAction(nameof(Index));
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
    }
}
