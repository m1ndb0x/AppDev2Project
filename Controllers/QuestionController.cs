using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppDev2Project.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace AppDev2Project.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class QuestionController : Controller
    {
        private readonly ExaminaDatabaseContext _context;
        private readonly ILogger<QuestionController> _logger;

        public QuestionController(ExaminaDatabaseContext context, ILogger<QuestionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Create(int examId)
        {
            try
            {
                var question = new QuestionViewModel
                {
                    ExamId = examId,
                    QuestionType = "multiple_choice",
                    ScoreWeight = 1.0,
                    Order = await _context.Questions.CountAsync(q => q.ExamId == examId) + 1
                };

                TempData["Info"] = "Add a new question to the exam.";
                return View(question);
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to initialize question creation.";
                return RedirectToAction("Edit", "Exam", new { id = examId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please check your input and try again.";
                return View(model);
            }

            try
            {
                var question = new Question
                {
                    ExamId = model.ExamId,
                    QuestionText = model.QuestionText,
                    QuestionType = model.QuestionType,
                    ScoreWeight = model.ScoreWeight,
                    Order = model.Order ?? 1
                };

                switch (model.QuestionType)
                {
                    case "multiple_choice":
                        if (string.IsNullOrEmpty(model.SelectedAnswer) || 
                            !Regex.IsMatch(model.SelectedAnswer.ToUpper(), "^[A-D]$"))
                        {
                            ModelState.AddModelError("SelectedAnswer", "Please select a valid answer (A-D)");
                            return View(model);
                        }
                        question.ChoiceA = model.ChoiceA;
                        question.ChoiceB = model.ChoiceB;
                        question.ChoiceC = model.ChoiceC;
                        question.ChoiceD = model.ChoiceD;
                        question.CorrectAnswer = model.SelectedAnswer.ToUpper();
                        break;

                    case "true_false":
                        if (string.IsNullOrEmpty(model.SelectedAnswer) || 
                            !new[] { "true", "false" }.Contains(model.SelectedAnswer.ToLower()))
                        {
                            ModelState.AddModelError("SelectedAnswer", "Please select either True or False");
                            return View(model);
                        }
                        question.CorrectAnswer = model.SelectedAnswer.ToLower();
                        break;

                    case "short_answer":
                        if (string.IsNullOrEmpty(model.CorrectAnswer))
                        {
                            ModelState.AddModelError("CorrectAnswer", "Please provide the correct answer");
                            return View(model);
                        }
                        question.CorrectAnswer = model.CorrectAnswer.Trim();
                        break;
                }

                _context.Questions.Add(question);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Question added successfully!";
                return RedirectToAction("Edit", "Exam", new { id = model.ExamId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to create question: {ex.Message}";
                return View(model);
            }
        }

        // GET: Edit Question
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var question = await _context.Questions
                    .Include(q => q.Exam)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (question == null)
                {
                    TempData["Error"] = "Question not found.";
                    return NotFound();
                }

                // Verify teacher ownership
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (question.Exam.TeacherId != int.Parse(userId))
                {
                    return Forbid();
                }

                TempData["Info"] = "Edit your question below.";
                return View(question);
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to load question for editing.";
                return RedirectToAction("Edit", "Exam", new { id = id });
            }
        }

        // POST: Edit Question
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ExamId,QuestionText,QuestionType,ChoiceA,ChoiceB,ChoiceC,ChoiceD,CorrectAnswer,ScoreWeight,Order")] Question question)
        {
            try
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
                        TempData["Success"] = "Question updated successfully!";
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
            catch (DbUpdateConcurrencyException)
            {
                TempData["Error"] = "Question was modified by another user.";
                return View(question);
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to update question.";
                return View(question);
            }
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
            try
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

                TempData["Success"] = "Question deleted successfully.";
                return RedirectToAction("Edit", "Exam", new { id = question.ExamId });
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to delete question.";
                return RedirectToAction("Edit", "Exam", new { id = id });
            }
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }
    }
}
