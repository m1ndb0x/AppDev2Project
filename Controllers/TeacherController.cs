using Microsoft.AspNetCore.Mvc;
using AppDev2Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AppDev2Project.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly ExaminaDatabaseContext _context;
        private readonly UserManager<User> _userManager;

        public TeacherController(ExaminaDatabaseContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private int GetCurrentTeacherId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User is not authenticated");
            
            return int.Parse(userIdClaim.Value);
        }

        // Dashboard View
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                
                var exams = await _context.Exams
                    .Include(e => e.CompletedExams)
                    .Include(e => e.Questions)  // Include questions
                    .Where(e => e.TeacherId == userId)
                    .ToListAsync();

                // Calculate active exams correctly by checking HasStarted property
                var activeExams = await _context.Exams
                    .Where(e => e.TeacherId == userId && e.HasStarted && !e.IsClosed)
                    .CountAsync();

                var recentExams = await _context.Exams
                    .Where(e => e.TeacherId == userId)
                    .OrderByDescending(e => e.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                ViewBag.TotalExams = exams.Count;
                ViewBag.ActiveExams = activeExams;  // Use the new activeExams count
                ViewBag.TotalQuestions = exams.Sum(e => e.Questions.Count);  // Calculate total questions
                ViewBag.RecentExams = recentExams;  // Set the RecentExams property
                ViewBag.RecentSubmissions = await _context.CompletedExams
                    .Include(ce => ce.Exam)
                    .Include(ce => ce.User)
                    .Where(ce => ce.Exam.TeacherId == userId)
                    .OrderByDescending(ce => ce.CompletedAt)
                    .Take(5)
                    .ToListAsync();

                TempData["Success"] = "Welcome to your teacher dashboard!";
                return View();
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to load dashboard. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        // Create Exam View
        public IActionResult CreateExam()
        {
            TempData["Info"] = "Create a new exam by filling out the form below.";
            var exam = new Exam
            {
                State = "draft",
                TeacherId = GetCurrentTeacherId()
            };
            return View(exam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExam(Exam exam)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    exam.CreatedAt = DateTime.Now;
                    exam.State = "draft";
                    exam.TeacherId = GetCurrentTeacherId();

                    _context.Exams.Add(exam);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Exam created successfully!";
                    return RedirectToAction(nameof(Dashboard));
                }
                catch (UnauthorizedAccessException)
                {
                    TempData["Error"] = "You are not authorized to create exams.";
                    return RedirectToAction("Login", "Account");
                }
                catch (Exception)
                {
                    TempData["Error"] = "Failed to create exam. Please try again.";
                }
            }
            TempData["Error"] = "Please check your input and try again.";
            return View(exam);
        }

        // Manage Exams View
        public IActionResult ManageExams()
        {
            return View("ManageExams");
        }

        // Grade Exams View
        public IActionResult GradeExams()
        {
            return View("GradeExams");
        }

        // Student Management Methods
        public async Task<IActionResult> Students()
        {
            var students = await _context.Users
                .Where(u => u.Role == "Student")
                .ToListAsync();
            
            return View("Students", students);
        }

        public async Task<IActionResult> AssignExam(string studentId)
        {
            var student = await _userManager.FindByIdAsync(studentId);
            if (student == null)
            {
                return NotFound();
            }

            // Get available exams for this teacher
            var teacherId = GetCurrentTeacherId();
            var availableExams = await _context.Exams
                .Where(e => e.TeacherId == teacherId)
                .ToListAsync();

            ViewBag.Student = student;
            return View(availableExams);
        }

        [HttpPost]
        public async Task<IActionResult> AssignExamToStudent(int examId, int studentId)
        {
            var exam = await _context.Exams
                .Include(e => e.AssignedStudents)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null) return NotFound();

            var student = await _userManager.FindByIdAsync(studentId.ToString());
            if (student == null) return NotFound();

            if (!exam.AssignedStudents.Any(s => s.Id == studentId))
            {
                exam.AssignedStudents.Add(student);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Students));
        }

        public async Task<IActionResult> CompletedExams()
        {
            try
            {
                var userId = GetCurrentTeacherId();
                var completedExams = await _context.CompletedExams
                    .Include(ce => ce.Exam)
                    .Include(ce => ce.User)
                    .Where(ce => ce.Exam.TeacherId == userId)
                    .OrderByDescending(ce => ce.CompletedAt)
                    .ToListAsync();

                if (!completedExams.Any())
                {
                    TempData["Info"] = "No completed exams found.";
                }
                else
                {
                    TempData["Success"] = "Viewing all completed exams.";
                }
                return View(completedExams);
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to load completed exams.";
                return RedirectToAction(nameof(Dashboard));
            }
        }

        public async Task<IActionResult> ReviewSubmission(int examId, int studentId)
        {
            try
            {
                var submission = await _context.CompletedExams
                    .Include(ce => ce.Exam)
                        .ThenInclude(e => e.Questions)
                    .Include(ce => ce.User)
                    .FirstOrDefaultAsync(ce => ce.ExamId == examId && ce.UserId == studentId);

                if (submission == null) return NotFound();

                // Get the student's answers
                var attempts = await _context.QuestionAttempt
                    .Where(qa => qa.ExamId == examId && qa.UserId == studentId)
                    .ToListAsync();

                ViewBag.Attempts = attempts;
                TempData["Info"] = "Reviewing student submission.";
                return View(submission);
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to load submission review.";
                return RedirectToAction(nameof(CompletedExams));
            }
        }
    }
}
