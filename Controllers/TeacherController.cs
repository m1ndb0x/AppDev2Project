using Microsoft.AspNetCore.Mvc;

namespace AppDev2Project.Controllers
{
    public class TeacherController : Controller
    {
        // Dashboard View
        public IActionResult Dashboard()
        {
            return View("Dashboard");
        }

        // Create Exam View
        public IActionResult CreateExam()
        {
            return View("CreateExam");
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
