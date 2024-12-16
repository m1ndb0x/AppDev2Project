using Microsoft.AspNetCore.Mvc;
using AppDev2Project.Models;

namespace AppDev2Project.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult StudentDashboard()
        {
            var model = new StudentDashboard
            {
                WelcomeMessage = "Hello, Student! Here's what's new:",
                RecentExams = new List<string> { "Math Exam", "Science Quiz", "History Test" }
            };

            return View("StudentDashboard", model);
        }
    }
}
