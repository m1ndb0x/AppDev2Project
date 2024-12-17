using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AppDev2Project.Models;
using Microsoft.AspNetCore.Authorization;

namespace AppDev2Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View("~/Views/Account/Register.cshtml");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    Role = "Student", //Default Role 
                    ProfilePictureUrl = GenerateDefaultPfp(model.Name) // Set default PFP

                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {model.Email} registered successfully.");

                    // Sign in the user
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("StudentDashboard", "Student");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View("Register");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View("~/Views/Account/Login.cshtml");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    _logger.LogInformation($"User {model.Email} logged in successfully.");

                    if (user.Role == "Teacher")
                    {
                        // Razor Page path for Teacher Dashboard
                        return RedirectToAction("Dashboard", "Teacher");
                    }
                    else if (user.Role == "Student")
                    {
                        // Razor Page path for Student Dashboard
                        return RedirectToAction("StudentDashboard", "Student");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                _logger.LogWarning($"Failed login attempt for {model.Email}.");
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View("~/Views/Account/Login.cshtml");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Login", "Account");
        }

        // Generate default profile picture URL with initials
        private string GenerateDefaultPfp(string name)
        {
            // Generate the initials-based image URL
            var initials = string.Join("", name.Split(' ').Select(w => w[0])).ToUpper();
            return $"https://ui-avatars.com/api/?name={initials}&background=random&color=fff";
        }
    }
}
