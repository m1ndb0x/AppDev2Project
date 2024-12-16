using Microsoft.AspNetCore.Mvc;
using AppDev2Project.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AppDev2Project.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly ExaminaDatabaseContext _context;

        public TeacherController(ExaminaDatabaseContext context)
        {
            _context = context;
        }

        private int GetCurrentTeacherId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User is not authenticated");
            
            return int.Parse(userIdClaim.Value);
        }

        // Dashboard View
        public IActionResult Dashboard()
        {
            return View("Dashboard");
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
    }
}
