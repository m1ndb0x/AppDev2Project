using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppDev2Project.Models;

public partial class Question
{
    public int Id { get; set; }

    [Required]
    public int ExamId { get; set; }

    [Required(ErrorMessage = "Question text is required")]
    public string QuestionText { get; set; } = null!;

    [Required(ErrorMessage = "Question type is required")]
    [RegularExpression("^(multiple_choice|short_answer)$", ErrorMessage = "Invalid question type")]
    public string QuestionType { get; set; } = "multiple_choice";

    public string? ChoiceA { get; set; }

    public string? ChoiceB { get; set; }

    public string? ChoiceC { get; set; }

    public string? ChoiceD { get; set; }

    [Required(ErrorMessage = "Correct answer is required")]
    [StringLength(255)]
    public string CorrectAnswer { get; set; } = null!;

    [Required(ErrorMessage = "Score weight is required")]
    [Range(0.5, 100.0, ErrorMessage = "Score weight must be between 0.5 and 100")]
    public double ScoreWeight { get; set; } = 1.0;

    public int? Order { get; set; }

    [ForeignKey("ExamId")]
    public virtual Exam? Exam { get; set; }  // Make nullable

    public virtual ICollection<QuestionAttempt> QuestionAttempt  { get; set; } = new List<QuestionAttempt>(); 
}
