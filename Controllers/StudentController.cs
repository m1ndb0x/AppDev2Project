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
            try
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
                    TempData["Error"] = "Student profile not found. Please contact support.";
                    return NotFound();
                }

                // Modified query to get all assigned exams that are active
                var availableExams = student.AssignedExams
                    .Where(e => e.HasStarted && 
                               !student.CompletedExams.Any(ce => ce.ExamId == e.Id) &&
                               e.StartedAt.HasValue &&
                               DateTime.Now < e.StartedAt.Value.AddMinutes(e.Duration))
                    .ToList();

                // Check for new exams
                var newExams = availableExams.Where(e => e.HasStarted && e.StartedAt?.AddMinutes(5) > DateTime.Now);
                if (newExams.Any())
                {
                    TempData["Info"] = $"You have {newExams.Count()} new exam(s) available!";
                }

                // Check for specific exam notifications
                if (TempData[$"NewExam_{userId}"] != null)
                {
                    TempData["Success"] = TempData[$"NewExam_{userId}"].ToString();
                }

                var examProgresses = await _context.ExamProgress
                    .Include(ep => ep.Exam)
                        .ThenInclude(e => e.Questions)
                    .Where(ep => ep.UserId == userId && !ep.IsCompleted)
                    .ToListAsync();

                var viewModel = new StudentDashboardViewModelV2
                {
                    Name = student.Name,
                    ProfilePictureUrl = student.ProfilePictureUrl,
                    Exams = availableExams,
                    CompletedExams = student.CompletedExams
                        .OrderByDescending(ce => ce.CompletedAt)
                        .ToList(),
                    ExamProgresses = examProgresses
                };

                TempData["Success"] = "Welcome back to your dashboard!";
                return View(viewModel);
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to load dashboard. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> ExamHistory()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var completedExams = await _context.CompletedExams
                    .Where(ce => ce.UserId == userId)
                    .Include(ce => ce.Exam)
                    .OrderByDescending(ce => ce.CompletedAt)
                    .ToListAsync();

                if (!completedExams.Any())
                {
                    TempData["Info"] = "You haven't completed any exams yet.";
                }
                else
                {
                    TempData["Success"] = "Here's your exam history.";
                }

                return View(completedExams);
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to load exam history. Please try again.";
                return RedirectToAction(nameof(Dashboard));
            }
        }

        public async Task<IActionResult> ViewExam(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var completedExam = await _context.CompletedExams
                    .Include(ce => ce.Exam)
                        .ThenInclude(e => e.Questions)
                    .FirstOrDefaultAsync(ce => ce.ExamId == id && ce.UserId == userId);

                if (completedExam == null)
                {
                    TempData["Error"] = "Exam result not found. It may have been deleted or you don't have permission to view it.";
                    return RedirectToAction(nameof(ExamHistory));
                }

                var percentage = (completedExam.TotalScore / completedExam.Exam.TotalScoreWeight) * 100;
                if (percentage >= 60)
                {
                    TempData["Success"] = $"Congratulations! You passed with {percentage:F1}%";
                }
                else
                {
                    TempData["Warning"] = $"You scored {percentage:F1}%. Keep practicing to improve!";
                }

                return View(completedExam);
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to load exam results. Please try again.";
                return RedirectToAction(nameof(ExamHistory));
            }
        }
    }
}
