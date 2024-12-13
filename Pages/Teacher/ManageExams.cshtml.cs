using Microsoft.AspNetCore.Mvc.RazorPages;
using AppDev2Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppDev2Project.Pages.Teacher
{
    public class ManageExamsModel : PageModel
    {
        private readonly ExaminaDatabaseContext _context;

        public ManageExamsModel(ExaminaDatabaseContext context)
        {
            _context = context;
        }

        public required IList<Exam> Exams { get; set; }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                // Handle the case where userId is null
                Exams = new List<Exam>();
                return;
            }
            Exams = await _context.Exams
                .Include(e => e.Teacher)
                .Where(e => e.TeacherId == int.Parse(userId))
                .ToListAsync();
        }
    }
}