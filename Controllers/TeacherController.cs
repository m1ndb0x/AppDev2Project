using Microsoft.AspNetCore.Mvc;

namespace AppDev2Project.Controllers
{
    public class TeacherController : Controller
    {
        public IActionResult Index()
        {
            return View("/Pages/Teacher/Index.cshtml");
        }
    }
}