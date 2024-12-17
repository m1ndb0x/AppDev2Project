using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppDev2Project.Models;
using AppDev2Project.Models.ViewModels;
using System.Security.Claims;

namespace AppDev2Project.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ExaminaDatabaseContext _context;

        public StudentController(UserManager<User> userManager, ExaminaDatabaseContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var student = await _context.Users
                .Include(u => u.AssignedExams)
                    .ThenInclude(e => e.Questions)
                .Include(u => u.CompletedExams)
                    .ThenInclude(ce => ce.Exam)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (student == null)
            {
                return NotFound();
            }

            // Get all assigned exams that haven't been completed yet
            var availableExams = student.AssignedExams
                .Where(e => !student.CompletedExams.Any(ce => ce.ExamId == e.Id))
                .OrderBy(e => e.AvailableFrom)
                .ToList();

            var viewModel = new StudentDashboardViewModelV2
            {
                Name = student.Name,
                ProfilePictureUrl = student.ProfilePictureUrl,
                Exams = availableExams,
                CompletedExams = student.CompletedExams.OrderByDescending(ce => ce.CompletedAt).ToList()
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
