using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AppDev2Project.Models;

namespace AppDev2Project.Pages.Teacher;

public class CreateExamModel : PageModel
{
    private readonly ExaminaDatabaseContext _context;

    public CreateExamModel(ExaminaDatabaseContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Exam Exam { get; set; } = new Exam();

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Exams.Add(Exam);
        _context.SaveChanges();

        return RedirectToPage("/Teacher/ManageExams");
    }
}
