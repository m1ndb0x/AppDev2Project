using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppDev2Project.Models;
using AppDev2Project.Models.ViewModels;
using System.Security.Claims;

namespace AppDev2Project.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ExaminaDatabaseContext _context;

        public StudentController(ExaminaDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var student = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (student == null)
            {
                return NotFound();
            }

            var viewModel = new StudentDashboardViewModel
            {
                Student = student,
                AvailableExams = await _context.Exams
                    .Where(e => e.State == "Complete" &&
                           e.AvailableFrom <= DateTime.Now &&
                           (!e.AvailableUntil.HasValue || e.AvailableUntil > DateTime.Now))
                    .Include(e => e.Questions)
                    .Where(e => !_context.CompletedExams
                        .Any(ce => ce.ExamId == e.Id && ce.UserId == userId))
                    .ToListAsync(),
                CompletedExams = await _context.CompletedExams
                    .Where(ce => ce.UserId == userId)
                    .Include(ce => ce.Exam)
                    .OrderByDescending(ce => ce.CompletedAt)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ExamHistory()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var completedExams = await _context.CompletedExams
                .Where(ce => ce.UserId == userId)
                .Include(ce => ce.Exam)
                .OrderByDescending(ce => ce.CompletedAt)
                .ToListAsync();

            return View(completedExams);
        }

        public async Task<IActionResult> ViewExam(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var completedExam = await _context.CompletedExams
                .Include(ce => ce.Exam)
                    .ThenInclude(e => e.Questions)
                .FirstOrDefaultAsync(ce => ce.ExamId == id && ce.UserId == userId);

            if (completedExam == null)
            {
                return NotFound();
            }

            return View(completedExam);
        }
    }
}
