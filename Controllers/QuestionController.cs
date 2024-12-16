using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppDev2Project.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppDev2Project.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class QuestionController : Controller
    {
        private readonly ExaminaDatabaseContext _context;

        public QuestionController(ExaminaDatabaseContext context)
        {
            _context = context;
        }

        public IActionResult Create(int examId)
        {
            var question = new Question { ExamId = examId };
            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExamId,QuestionText,QuestionType,ChoiceA,ChoiceB,ChoiceC,ChoiceD,CorrectAnswer,ScoreWeight,Order")] Question question)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var exam = await _context.Exams.FindAsync(question.ExamId);
                    if (exam == null)
                    {
                        return NotFound();
                    }

                    // Validate teacher ownership
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (exam.TeacherId != int.Parse(userId))
                    {
                        return Forbid();
                    }

                    // Validate multiple choice questions
                    if (question.QuestionType == "multiple_choice")
                    {
                        if (string.IsNullOrEmpty(question.ChoiceA) || 
                            string.IsNullOrEmpty(question.ChoiceB) || 
                            string.IsNullOrEmpty(question.ChoiceC) || 
                            string.IsNullOrEmpty(question.ChoiceD))
                        {
                            ModelState.AddModelError("", "All choices must be provided for multiple choice questions");
                            return View(question);
                        }

                        // Validate correct answer is A, B, C, or D
                        if (!new[] { "A", "B", "C", "D" }.Contains(question.CorrectAnswer.ToUpper()))
                        {
                            ModelState.AddModelError("CorrectAnswer", "Correct answer must be A, B, C, or D for multiple choice questions");
                            return View(question);
                        }
                    }

                    // Set order if not specified
                    if (!question.Order.HasValue)
                    {
                        var maxOrder = await _context.Questions
                            .Where(q => q.ExamId == question.ExamId)
                            .MaxAsync(q => (int?)q.Order) ?? 0;
                        question.Order = maxOrder + 1;
                    }

                    _context.Questions.Add(question);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Edit", "Exam", new { id = question.ExamId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the question. Please try again.");
                    // Log the exception details here
                }
            }

            // If we got this far, something failed, redisplay form
            return View(question);
        }
    }
}
