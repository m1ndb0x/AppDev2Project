using Microsoft.AspNetCore.Mvc.RazorPages;
using AppDev2Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppDev2Project.Pages.Teacher
{
    public class GradeExamsModel : PageModel
    {
        private readonly ExaminaDatabaseContext _context;

        public GradeExamsModel(ExaminaDatabaseContext context)
        {
            _context = context;
        }

        public IList<CompletedExam> CompletedExams { get; set; }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                CompletedExams = new List<CompletedExam>();
                return;
            }
            CompletedExams = await _context.CompletedExams
                .Include(e => e.Exam)
                .Include(e => e.User)
                .Where(e => e.Exam.TeacherId == int.Parse(userId))
                .ToListAsync();
        }
    }
}
