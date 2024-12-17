using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AppDev2Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

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
                    UserName = model.Email, // Use email as username for Identity
                    Email = model.Email,
                    Name = model.Name,      // Name can contain spaces
                    Role = model.Role // Assuming you added Role to RegisterViewModel
                    ProfilePictureUrl = GenerateDefaultPfp(model.Name) // Set default PFP

                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {model.Email} registered successfully.");

                    // Add role claim
                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, user.Role));
                
                    // Add user ID claim
                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

                    // Sign in the user
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
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
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        // Create claims principal with additional claims
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Role, user.Role)
                        };

                        await _signInManager.SignInWithClaimsAsync(user, model.RememberMe, claims);

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
                }
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View("~/Views/Account/Login.cshtml");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Login", "Account");
        }
    }
}
