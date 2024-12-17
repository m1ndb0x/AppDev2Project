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
            var teacherId = GetCurrentTeacherId();
            
            // Get exam statistics
            ViewBag.TotalExams = await _context.Exams
                .Where(e => e.TeacherId == teacherId)
                .CountAsync();

            ViewBag.ActiveExams = await _context.Exams
                .Where(e => e.TeacherId == teacherId && e.State == "Complete")
                .CountAsync();

            ViewBag.TotalQuestions = await _context.Questions
                .Include(q => q.Exam)
                .Where(q => q.Exam.TeacherId == teacherId)
                .CountAsync();

            // Get recent exams
            ViewBag.RecentExams = await _context.Exams
                .Where(e => e.TeacherId == teacherId)
                .OrderByDescending(e => e.CreatedAt)
                .Take(5)
                .Select(e => new
                {
                    Id = e.Id,
                    Title = e.Title,
                    Subject = e.Subject,
                    State = e.State,
                    QuestionsCount = e.Questions.Count
                })
                .ToListAsync();

            return View();
        }

        // Create Exam View
        public IActionResult CreateExam()
        {
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
                    return RedirectToAction(nameof(Dashboard));
                }
                catch (UnauthorizedAccessException)
                {
                    return RedirectToAction("Login", "Account");
                }
            }
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
    }
}
