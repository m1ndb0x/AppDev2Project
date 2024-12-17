using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppDev2Project.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using AppDev2Project.Models.ViewModels;

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
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var isTeacher = User.IsInRole("Teacher");

            var examsQuery = _context.Exams
                .Include(e => e.Teacher)
                .Include(e => e.Questions)
                .Include(e => e.AssignedStudents)  // Include assigned students
                .AsNoTracking();

            // Filter based on role
            if (isTeacher)
            {
                // Teachers only see their own created exams
                examsQuery = examsQuery.Where(e => e.TeacherId == userId);
            }
            else
            {
                // Students see exams assigned to them that are within the available time window
                examsQuery = examsQuery
                    .Where(e => e.AssignedStudents.Any(s => s.Id == userId) &&
                               e.AvailableFrom <= DateTime.Now &&
                               (!e.AvailableUntil.HasValue || e.AvailableUntil > DateTime.Now));
            }

            var exams = await examsQuery
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
                
            return View(exams);
        }

        // View Exam Details
        public async Task<IActionResult> Details(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.Teacher)
                .Include(e => e.Questions)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exam == null)
            {
                return NotFound();
            }

            // Force collection load and sorting
            var questions = await _context.Questions
                .Where(q => q.ExamId == id)
                .OrderBy(q => q.Order)
                .ToListAsync();

            exam.Questions = questions;

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
        public async Task<IActionResult> Create([Bind("Title,Description,Subject,TotalScoreWeight,AvailableFrom,AvailableUntil,Duration")] Exam exam)
        {
            // Remove Teacher validation since we'll set it manually
            ModelState.Remove("Teacher");
            ModelState.Remove("TeacherId");

            if (!ModelState.IsValid)
            {
                return View(exam);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError("", "Teacher ID not found. Please log in again.");
                    return View(exam);
                }

                var teacher = await _context.Users.FindAsync(int.Parse(userId));
                if (teacher == null)
                {
                    ModelState.AddModelError("", "Teacher not found. Please log in again.");
                    return View(exam);
                }

                // Set the teacher relationship
                exam.TeacherId = teacher.Id;
                exam.Teacher = teacher;
                exam.State = "Incomplete";
                exam.CreatedAt = DateTime.UtcNow;
                exam.Questions = new List<Question>();

                if (exam.AvailableFrom == default)
                {
                    exam.AvailableFrom = DateTime.UtcNow;
                }

                _context.Exams.Add(exam);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Edit), new { id = exam.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating exam: {ex.Message}");
                return View(exam);
            }
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Subject,TotalScoreWeight,State,TeacherId,CreatedAt")] Exam exam)
        {
            if (id != exam.Id)
            {
                return NotFound();
            }

            // Remove TeacherId from validation since it's a hidden field
            ModelState.Remove("Teacher");

            if (ModelState.IsValid)
            {
                try
                {
                    // Attach and mark entity as modified
                    _context.Entry(exam).State = EntityState.Modified;
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

            // If we got this far, something failed, redisplay form
            var reloadedExam = await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == id);
            return View(reloadedExam);
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

        // Take Exam (GET)
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> TakeExam(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var exam = await _context.Exams
                .Include(e => e.Teacher)
                .Include(e => e.Questions)
                .Include(e => e.AssignedStudents)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exam == null)
            {
                return NotFound();
            }

            // Check if student is assigned to this exam
            if (!exam.AssignedStudents.Any(s => s.Id == userId))
            {
                return Forbid("You are not assigned to this exam.");
            }

            // Check if exam is available
            if (exam.AvailableFrom > DateTime.Now || 
                (exam.AvailableUntil.HasValue && exam.AvailableUntil < DateTime.Now))
            {
                return BadRequest("This exam is not available at this time.");
            }

            // Check if student has already completed this exam
            var alreadyCompleted = await _context.CompletedExams
                .AnyAsync(ce => ce.ExamId == id && ce.UserId == userId);

            if (alreadyCompleted)
            {
                return RedirectToAction("ViewResult", new { id = exam.Id });
            }

            return View(exam);
        }

        // Submit Exam (POST)
        [HttpPost]
        [Authorize(Roles = "Student")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitExam(int examId, Dictionary<int, QuestionAnswerViewModel> questions)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Check if exam exists and is complete
            var exam = await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null || exam.State != "Complete")
            {
                return NotFound();
            }

            // Check for existing submission
            var existingSubmission = await _context.CompletedExams
                .AnyAsync(ce => ce.ExamId == examId && ce.UserId == userId);

            if (existingSubmission)
            {
                return BadRequest("You have already submitted this exam.");
            }

            // Create completed exam record
            var completedExam = new CompletedExam
            {
                ExamId = examId,
                UserId = userId,
                CompletedAt = DateTime.Now,
                GradedAt = DateTime.Now,
                TotalScore = 0.0 // Will be updated after processing questions
            };

            _context.CompletedExams.Add(completedExam);

            // Calculate total score
            double totalScore = 0;
            foreach (var question in exam.Questions)
            {
                if (questions.TryGetValue(question.Id, out var answer))
                {
                    var attempt = new QuestionAttempt
                    {
                        QuestionId = question.Id,
                        UserId = userId,
                        AnswerText = answer.Answer,
                        IsGraded = true,
                        Grade = answer.Answer == question.CorrectAnswer ? question.ScoreWeight : 0,
                        SubmittedAt = DateTime.Now
                    };
                    _context.QuestionAttempt.Add(attempt);
                    
                    // Add score calculation here
                    if (answer.Answer == question.CorrectAnswer)
                    {
                        totalScore += question.ScoreWeight;
                    }
                }
            }

            completedExam.TotalScore = totalScore;
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewResult", new { id = examId });
        }

        // View Exam Result
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> ViewResult(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var completedExam = await _context.CompletedExams
                .Include(ce => ce.Exam)
                .Include(ce => ce.Exam.Questions)
                .Include(ce => ce.User)
                .FirstOrDefaultAsync(ce => ce.ExamId == id && ce.UserId == userId);

            if (completedExam == null)
            {
                return NotFound();
            }

            return View(completedExam);
        }
    }
}
