using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppDev2Project.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using AppDev2Project.Models.ViewModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
                               e.HasStarted);
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
        public async Task<IActionResult> Create([Bind("Title,Description,Subject,TotalScoreWeight,Duration")] Exam exam)
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
                exam.HasStarted = false;
                exam.StartedAt = null;


                _context.Exams.Add(exam);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Exam created successfully!";
                return RedirectToAction(nameof(Edit), new { id = exam.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating exam: {ex.Message}";
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
                TempData["Error"] = "Exam not found.";
                return NotFound();
            }

            // Check if user is the teacher who created the exam
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (exam.TeacherId != int.Parse(userId))
            {
                TempData["Error"] = "You are not authorized to edit this exam.";
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
                    TempData["Success"] = "Exam updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamExists(exam.Id))
                    {
                        TempData["Error"] = "Exam no longer exists.";
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            TempData["Error"] = "Please correct the errors in the form.";
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
                TempData["Error"] = "Exam not found.";
                return NotFound();
            }

            // Only allow deletion by the teacher who created the exam
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (exam.TeacherId != int.Parse(userId))
            {
                TempData["Error"] = "You are not authorized to delete this exam.";
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
                TempData["Error"] = "You are not authorized to delete this exam.";
                return Forbid();
            }

            try
            {
                _context.Exams.Remove(exam);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Exam deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to delete exam. Please try again.";
                return View(exam);
            }
        }

        // Take Exam (GET)
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> TakeExam(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var exam = await _context.Exams
                .Include(e => e.Questions)
                .Include(e => e.AssignedStudents)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exam == null || !exam.HasStarted)
            {
                TempData["Error"] = "Exam is not available";
                return RedirectToAction("Dashboard", "Student");
            }

            var progress = await _context.ExamProgress
                .FirstOrDefaultAsync(ep => ep.ExamId == id && ep.UserId == userId);

            if (progress == null)
            {
                progress = new ExamProgress
                {
                    ExamId = id,
                    UserId = userId,
                    StartedAt = DateTime.Now,
                    LastUpdated = DateTime.Now,
                    SavedAnswers = "{}",
                    IsCompleted = false,
                    IsActive = true
                };
                _context.ExamProgress.Add(progress);
                await _context.SaveChangesAsync();
            }
            else if (!progress.IsActive)
            {
                TempData["Error"] = "You have been removed from this exam.";
                return RedirectToAction("Dashboard", "Student");
            }

            // Check if time has expired
            var timeElapsed = DateTime.Now - progress.StartedAt;
            if (timeElapsed.TotalMinutes > exam.Duration)
            {
                await AutoSubmitExam(id, userId);
                return RedirectToAction("ViewResult", new { id });
            }

            ViewData["ExamProgress"] = progress;
            ViewData["TimeLeft"] = exam.Duration - timeElapsed.TotalMinutes;
            return View(exam);
        }

        // Add new helper method for auto-submission
        private async Task AutoSubmitExam(int examId, int userId)
        {
            var progress = await _context.ExamProgress
                .Include(ep => ep.Exam)
                .FirstOrDefaultAsync(ep => ep.ExamId == examId && ep.UserId == userId);

            if (progress != null && !progress.IsCompleted)
            {
                progress.IsCompleted = true;
                progress.IsActive = false;

                // Calculate score from saved answers
                var answers = JsonConvert.DeserializeObject<Dictionary<string, string>>(progress.SavedAnswers ?? "{}");
                var questions = await _context.Questions.Where(q => q.ExamId == examId).ToListAsync();
                double totalScore = 0;

                foreach (var question in questions)
                {
                    if (answers.TryGetValue(question.Id.ToString(), out var answer))
                    {
                        if (answer == question.CorrectAnswer)
                        {
                            totalScore += question.ScoreWeight;
                        }
                    }
                }

                var completedExam = new CompletedExam
                {
                    ExamId = examId,
                    UserId = userId,
                    CompletedAt = DateTime.Now,
                    IsCompleted = true,
                    TotalScore = totalScore
                };

                _context.CompletedExams.Add(completedExam);
                await _context.SaveChangesAsync();
            }
        }

        // Add new endpoint to save progress
        [HttpPost]
        [Authorize(Roles = "Student")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProgress([FromBody] ProgressSaveModel model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }

                // Debug logging
               

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var progress = await _context.ExamProgress
                    .FirstOrDefaultAsync(ep => ep.ExamId == model.ExamId && ep.UserId == userId && !ep.IsCompleted);

                if (progress == null)
                {
                    progress = new ExamProgress
                    {
                        ExamId = model.ExamId,
                        UserId = userId,
                        StartedAt = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow,
                        IsCompleted = false,
                        SavedAnswers = model.Answers ?? "{}"
                    };
                    _context.ExamProgress.Add(progress);
                   
                }
                else
                {
                    progress.LastUpdated = DateTime.UtcNow;
                    progress.SavedAnswers = model.Answers ?? "{}";
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Progress saved successfully" });
            }
            catch (Exception ex)
            {
              
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Submit Exam (POST)
        [HttpPost]
        [Authorize(Roles = "Student")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitExam(int examId, Dictionary<int, QuestionAnswerViewModel> questions)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            // Get exam and progress
            var exam = await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null)
                return NotFound();

            var progress = await _context.ExamProgress
                .FirstOrDefaultAsync(ep => ep.ExamId == examId && ep.UserId == userId);

            if (progress == null || progress.IsCompleted)
                return BadRequest("Exam progress not found or already completed.");

            try
            {
                // Calculate total score
                double totalScore = 0;
                var questionAttempts = new List<QuestionAttempt>();

                foreach (var question in exam.Questions)
                {
                    if (questions.TryGetValue(question.Id, out var answer))
                    {
                        var isCorrect = answer.Answer == question.CorrectAnswer;
                        var score = isCorrect ? question.ScoreWeight : 0;
                        totalScore += score;

                        questionAttempts.Add(new QuestionAttempt
                        {
                            QuestionId = question.Id,
                            UserId = userId,
                            ExamId = examId,
                            AnswerText = answer.Answer,
                            IsGraded = true,
                            Grade = score,
                            SubmittedAt = DateTime.Now
                        });
                    }
                }

                // Create completed exam record
                var completedExam = new CompletedExam
                {
                    ExamId = examId,
                    UserId = userId,
                    TotalScore = totalScore,
                    CompletedAt = DateTime.Now,
                    GradedAt = DateTime.Now,
                    IsCompleted = true
                };

                // Mark progress as completed
                progress.IsCompleted = true;
                progress.IsActive = false;

                // Save everything
                _context.QuestionAttempt.AddRange(questionAttempts);
                _context.CompletedExams.Add(completedExam);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Exam submitted successfully!";
                return RedirectToAction("ViewResult", new { id = examId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while submitting your exam.";
                return RedirectToAction("TakeExam", new { id = examId });
            }
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
                TempData["Error"] = "Exam result not found.";
                return NotFound();
            }

            TempData["Success"] = "Here are your exam results.";
            return View(completedExam);
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TrackProgress(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.AssignedStudents)
                .Include(e => e.Questions)
                .Include(e => e.StudentProgress)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exam == null) return NotFound();

            var studentProgress = new Dictionary<int, StudentProgressInfo>();
            foreach (var student in exam.AssignedStudents)
            {
                var progress = exam.StudentProgress.FirstOrDefault(p => p.UserId == student.Id);
                if (progress != null)
                {
                    var answeredCount = 0;
                    try
                    {
                        if (!string.IsNullOrEmpty(progress.SavedAnswers))
                        {
                            var answers = JsonConvert.DeserializeObject<Dictionary<string, string>>(progress.SavedAnswers);
                            answeredCount = answers?.Count ?? 0;
                        }
                    }
                    catch (JsonException)
                    {
                        answeredCount = 0;
                    }

                    var timeRemaining = progress.StartedAt.AddMinutes(exam.Duration) - DateTime.UtcNow;
                    
                    studentProgress[student.Id] = new StudentProgressInfo
                    {
                        TimeRemaining = Math.Max(0, timeRemaining.TotalMinutes),
                        LastSaved = progress.LastUpdated.HasValue ? progress.LastUpdated.Value : progress.StartedAt,
                        CompletedQuestions = answeredCount
                    };
                }
                else
                {
                    studentProgress[student.Id] = new StudentProgressInfo
                    {
                        TimeRemaining = exam.Duration,
                        LastSaved = DateTime.MinValue,
                        CompletedQuestions = 0
                    };
                }
            }

            var viewModel = new ExamProgressTrackingViewModel
            {
                Exam = exam,
                StudentProgress = studentProgress
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> StartExam(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.AssignedStudents)
                .FirstOrDefaultAsync(e => e.Id == id);
                
            if (exam == null) return NotFound();

            if (!exam.AssignedStudents.Any())
            {
                TempData["Error"] = "Cannot start exam without any assigned students.";
                return RedirectToAction(nameof(TrackProgress), new { id });
            }

            if (exam.HasStarted)
            {
                TempData["Error"] = "Exam is already running.";
                return RedirectToAction(nameof(TrackProgress), new { id });
            }

            exam.HasStarted = true;
            exam.StartedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Exam started successfully with {exam.AssignedStudents.Count} student(s).";
            return RedirectToAction(nameof(TrackProgress), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> EndExam(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.StudentProgress)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exam == null) return NotFound();

            exam.HasStarted = false;
            exam.StartedAt = null;
            foreach (var progress in exam.StudentProgress.Where(sp => !sp.IsCompleted))
            {
                await AutoSubmitExam(exam.Id, progress.UserId);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<JsonResult> GetExamStatus(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.StudentProgress)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exam == null) return Json(new { error = "Exam not found" });

            return Json(new { 
                hasStarted = exam.HasStarted,
                startedAt = exam.StartedAt,
                studentCount = exam.StudentProgress.Count
            });
        }

        private async Task<TimeSpan> GetRemainingTime(int examId, int userId)
        {
            var progress = await _context.ExamProgress
                .FirstOrDefaultAsync(ep => ep.ExamId == examId && ep.UserId == userId);
                
            if (progress == null) return TimeSpan.Zero;

            var exam = await _context.Exams.FindAsync(examId);
            if (exam == null) return TimeSpan.Zero;

            var elapsed = DateTime.Now - progress.StartedAt;
            var remaining = TimeSpan.FromMinutes(exam.Duration) - elapsed;
            
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> KickStudent(int examId, int studentId)
        {
            var exam = await _context.Exams
                .Include(e => e.StudentProgress)
                .Include(e => e.AssignedStudents)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null) return NotFound();

            // Find and remove the student from assigned students
            var student = exam.AssignedStudents.FirstOrDefault(s => s.Id == studentId);
            if (student != null)
            {
                exam.AssignedStudents.Remove(student);
            }

            // Mark their progress as inactive and completed
            var progress = exam.StudentProgress.FirstOrDefault(p => p.UserId == studentId);
            if (progress != null)
            {
                progress.IsActive = false;
                progress.IsCompleted = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TrackProgress), new { id = examId });
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateTime(int examId, int studentId, double newTime)
        {
            var exam = await _context.Exams.FindAsync(examId);
            if (exam == null) return NotFound();

            var progress = await _context.ExamProgress
                .FirstOrDefaultAsync(ep => ep.ExamId == examId && ep.UserId == studentId);

            if (progress == null) return NotFound();

            // Simple calculation: set start time based on how much time we want them to have left
            progress.StartedAt = DateTime.Now.AddMinutes(-exam.Duration + newTime);
            
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Time updated successfully. Student now has {newTime} minutes remaining.";

            return RedirectToAction(nameof(TrackProgress), new { id = examId });
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> ExtendTimeForAll(int examId, int additionalMinutes)
        {
            var exam = await _context.Exams
                .Include(e => e.StudentProgress)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null) return NotFound();

            foreach (var progress in exam.StudentProgress.Where(p => !p.IsCompleted))
            {
                // Add additional time by adjusting the start time
                progress.StartedAt = progress.StartedAt.AddMinutes(-additionalMinutes);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Extended exam time by {additionalMinutes} minutes for all active students.";
            return RedirectToAction(nameof(TrackProgress), new { id = examId });
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> ExtendTimeForStudent(int examId, int studentId, int additionalMinutes)
        {
            var progress = await _context.ExamProgress
                .FirstOrDefaultAsync(ep => ep.ExamId == examId && ep.UserId == studentId && !ep.IsCompleted);

            if (progress == null) return NotFound();

            // Add additional time by adjusting the start time
            progress.StartedAt = progress.StartedAt.AddMinutes(-additionalMinutes);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Extended time for student successfully.";
            return RedirectToAction(nameof(TrackProgress), new { id = examId });
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<JsonResult> GetAvailableStudents(int examId)
        {
            var exam = await _context.Exams
                .Include(e => e.AssignedStudents)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null) return Json(new { error = "Exam not found" });

            var availableStudents = await _context.Users
                .Where(u => u.Role == "Student" && !exam.AssignedStudents.Contains(u))
                .Select(u => new { 
                    id = u.Id, 
                    name = u.Name, 
                    email = u.Email,
                    profilePictureUrl = u.ProfilePictureUrl ?? "/images/default-avatar.png"
                })
                .ToListAsync();

            return Json(availableStudents);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignStudent(int examId, int studentId)
        {
            var exam = await _context.Exams
                .Include(e => e.AssignedStudents)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null) return NotFound();
            if (exam.HasStarted)
            {
                TempData["Error"] = "Cannot assign students to an exam that has already started.";
                return RedirectToAction(nameof(TrackProgress), new { id = examId });
            }

            var student = await _context.Users.FindAsync(studentId);
            if (student == null) return NotFound();

            if (!exam.AssignedStudents.Any(s => s.Id == studentId))
            {
                exam.AssignedStudents.Add(student);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Student {student.Name} assigned successfully.";
            }

            return RedirectToAction(nameof(TrackProgress), new { id = examId });
        }
    }
}
