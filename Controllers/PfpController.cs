using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppDev2Project.Models;

namespace AppDev2Project.Controllers
{
    public class PfpController : Controller
    {
        private readonly ExaminaDatabaseContext _dbContext;

        public PfpController(ExaminaDatabaseContext dbContext)
        {
            _dbContext = dbContext;
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
    }
}
