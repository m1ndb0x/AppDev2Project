using System.ComponentModel.DataAnnotations;

namespace AppDev2Project.Models;

public class QuestionViewModel
{
    public int Id { get; set; }
    public int ExamId { get; set; }

    [Required(ErrorMessage = "Question text is required")]
    public string QuestionText { get; set; } = null!;

    [Required(ErrorMessage = "Question type is required")]
    [RegularExpression("^(multiple_choice|true_false|short_answer)$", ErrorMessage = "Invalid question type")]
    public string QuestionType { get; set; } = "multiple_choice";

    // Multiple choice options
    public string? ChoiceA { get; set; }
    public string? ChoiceB { get; set; }
    public string? ChoiceC { get; set; }
    public string? ChoiceD { get; set; }

    // Answer fields
    public string? SelectedAnswer { get; set; }
    public string? CorrectAnswer { get; set; }

    [Required(ErrorMessage = "Score weight is required")]
    [Range(0.5, 100.0, ErrorMessage = "Score weight must be between 0.5 and 100")]
    public double ScoreWeight { get; set; } = 1.0;

    public int? Order { get; set; }
}
