using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppDev2Project.Models;
using System.Threading.Tasks;
using System.Linq;

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
            var exams = _context.Exams.ToList();
            return View(exams);
        }

        // View Exam Details
        public IActionResult Details(int id)
        {
            var exam = _context.Exams.FirstOrDefault(e => e.Id == id);
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
        public async Task<IActionResult> Create(Exam exam)
        {
            if (ModelState.IsValid)
            {
                _context.Exams.Add(exam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(exam);
        }

        // Edit Exam (GET)
        [Authorize(Roles = "Teacher")]
        public IActionResult Edit(int id)
        {
            var exam = _context.Exams.FirstOrDefault(e => e.Id == id);
            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        // Edit Exam (POST)
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Exam exam)
        {
            if (ModelState.IsValid)
            {
                _context.Exams.Update(exam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(exam);
        }

        // Delete Exam (GET Confirmation)
        [Authorize(Roles = "Teacher")]
        public IActionResult Delete(int id)
        {
            var exam = _context.Exams.FirstOrDefault(e => e.Id == id);
            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        // Delete Exam (POST)
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exam = _context.Exams.FirstOrDefault(e => e.Id == id);
            if (exam != null)
            {
                _context.Exams.Remove(exam);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
