using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppDev2Project.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Logging; // Add this at the top

namespace AppDev2Project.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class QuestionController : Controller
    {
        private readonly ExaminaDatabaseContext _context;
        private readonly ILogger<QuestionController> _logger; // Add logger

        public QuestionController(ExaminaDatabaseContext context, ILogger<QuestionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Create(int examId)
        {
            var exam = _context.Exams.Find(examId);
            if (exam == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (exam.TeacherId != int.Parse(userId))
            {
                return Forbid();
            }

            var maxOrder = _context.Questions
                .Where(q => q.ExamId == examId)
                .Max(q => (int?)q.Order) ?? 0;

            var question = new Question { 
                ExamId = examId,
                Order = maxOrder + 1
            };
            
            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExamId,QuestionText,QuestionType,ChoiceA,ChoiceB,ChoiceC,ChoiceD,CorrectAnswer,ScoreWeight,Order")] Question question)
        {
            try
            {
                _logger.LogInformation($"Attempting to create question for exam {question.ExamId}");

                // Remove Exam from ModelState since it's a navigation property
                ModelState.Remove("Exam");

                // Log all model state errors
                if (!ModelState.IsValid)
                {
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            _logger.LogWarning($"Model validation error: {error.ErrorMessage}");
                        }
                    }
                    return View(question);
                }

                var exam = await _context.Exams.FindAsync(question.ExamId);
                if (exam == null)
                {
                    _logger.LogWarning($"Exam {question.ExamId} not found");
                    return NotFound("Exam not found");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (exam.TeacherId != int.Parse(userId))
                {
                    _logger.LogWarning($"User {userId} not authorized for exam {question.ExamId}");
                    return Forbid();
                }

                // Set default values
                if (!question.Order.HasValue)
                {
                    question.Order = await _context.Questions
                        .Where(q => q.ExamId == question.ExamId)
                        .MaxAsync(q => (int?)q.Order) ?? 0 + 1;
                }

                if (question.ScoreWeight <= 0)
                {
                    question.ScoreWeight = 1.0;
                }

                // Process based on question type
                if (question.QuestionType == "multiple_choice")
                {
                    question.CorrectAnswer = question.CorrectAnswer.ToUpper();
                }
                else
                {
                    question.ChoiceA = null;
                    question.ChoiceB = null;
                    question.ChoiceC = null;
                    question.ChoiceD = null;
                }

                _context.Questions.Add(question);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Question created successfully for exam {question.ExamId}");
                return RedirectToAction("Edit", "Exam", new { id = question.ExamId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question");
                ModelState.AddModelError("", "Failed to create question. Please try again.");
                return View(question);
            }
        }

        // GET: Edit Question
        public async Task<IActionResult> Edit(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Exam)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            // Verify teacher ownership
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (question.Exam.TeacherId != int.Parse(userId))
            {
                return Forbid();
            }

            return View(question);
        }

        // POST: Edit Question
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ExamId,QuestionText,QuestionType,ChoiceA,ChoiceB,ChoiceC,ChoiceD,CorrectAnswer,ScoreWeight,Order")] Question question)
        {
            if (id != question.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Exam");

            if (ModelState.IsValid)
            {
                try
                {
                    var exam = await _context.Exams.FindAsync(question.ExamId);
                    if (exam == null)
                    {
                        return NotFound("Exam not found");
                    }

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (exam.TeacherId != int.Parse(userId))
                    {
                        return Forbid();
                    }

                    // Handle multiple choice validation
                    if (question.QuestionType == "multiple_choice")
                    {
                        question.CorrectAnswer = question.CorrectAnswer.ToUpper();
                    }
                    else
                    {
                        question.ChoiceA = null;
                        question.ChoiceB = null;
                        question.ChoiceC = null;
                        question.ChoiceD = null;
                    }

                    _context.Update(question);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Edit", "Exam", new { id = question.ExamId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View(question);
        }

        // GET: Delete Question
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Exam)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (question.Exam.TeacherId != int.Parse(userId))
            {
                return Forbid();
            }

            return View(question);
        }

        // POST: Delete Question
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Exam)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (question.Exam.TeacherId != int.Parse(userId))
            {
                return Forbid();
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", "Exam", new { id = question.ExamId });
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }
    }
}
