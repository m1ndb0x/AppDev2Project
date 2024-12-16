using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppDev2Project.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

namespace AppDev2Project.Controllers
{
    [Authorize]
    public class ExamController : Controller
    {
        private readonly ExaminaDatabaseContext _context;

        public ExamController(ExaminaDatabaseContext context)
        {
            _context = context;
        }

        // List All Exams (For Students or Teachers)
        public IActionResult Index()
        {
            var exams = _context.Exams
                .Include(e => e.Teacher)
                .ToList();
            return View(exams);
        }

        // View Exam Details
        public IActionResult Details(int id)
        {
            var exam = _context.Exams
                .Include(e => e.Teacher)
                .Include(e => e.Questions)
                .FirstOrDefault(e => e.Id == id);
            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        // Create Exam (GET)
        [Authorize(Roles = "Teacher")]
        public IActionResult Create()
        {
            return View();
        }

        // Create Exam (POST)
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Exam exam)  // Change to [FromForm]
        {
            // Remove TeacherId from ModelState since we'll set it programmatically
            ModelState.Remove("TeacherId");
            ModelState.Remove("Teacher");
            
            if (ModelState.IsValid)
            {
                // Get current user's ID from claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null && User.IsInRole("Teacher"))
                {
                    exam.TeacherId = int.Parse(userId);
                    exam.State = "Incomplete";
                    exam.CreatedAt = DateTime.Now;

                    _context.Exams.Add(exam);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return Forbid();
            }
            return View(exam);
        }

        // Edit Exam (GET)
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.Questions)  // Include questions
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exam == null)
            {
                return NotFound();
            }

            // Check if user is the teacher who created the exam
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (exam.TeacherId != int.Parse(userId))
            {
                return Forbid();
            }

            return View(exam);
        }

        // Edit Exam (POST)
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Subject,TotalScoreWeight,State")] Exam exam)
        {
            if (id != exam.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingExam = await _context.Exams
                        .Include(e => e.Questions)
                        .FirstOrDefaultAsync(e => e.Id == id);

                    if (existingExam == null)
                    {
                        return NotFound();
                    }

                    // Check if user is the teacher who created the exam
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (existingExam.TeacherId != int.Parse(userId))
                    {
                        return Forbid();
                    }

                    // Only allow Complete state if there are questions
                    if (exam.State == "Complete" && !existingExam.Questions.Any())
                    {
                        ModelState.AddModelError("State", "Cannot mark exam as Complete without questions");
                        return View(existingExam);
                    }

                    existingExam.Title = exam.Title;
                    existingExam.Description = exam.Description;
                    existingExam.Subject = exam.Subject;
                    existingExam.TotalScoreWeight = exam.TotalScoreWeight;
                    existingExam.State = exam.State;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamExists(exam.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(exam);
        }

        private bool ExamExists(int id)
        {
            return _context.Exams.Any(e => e.Id == id);
        }

        // Delete Exam (GET Confirmation)
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.Questions)
                .Include(e => e.CompletedExams)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exam == null)
            {
                return NotFound();
            }

            // Only allow deletion by the teacher who created the exam
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (exam.TeacherId != int.Parse(userId))
            {
                return Forbid();
            }

            return View(exam);
        }

        // Delete Exam (POST)
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.Questions)
                .Include(e => e.CompletedExams)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exam == null)
            {
                return NotFound();
            }

            // Only allow deletion by the teacher who created the exam
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (exam.TeacherId != int.Parse(userId))
            {
                return Forbid();
            }

            try
            {
                _context.Exams.Remove(exam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                // Log the error
                ModelState.AddModelError("", "Unable to delete exam. Please try again.");
                return View(exam);
            }
        }
    }
}
