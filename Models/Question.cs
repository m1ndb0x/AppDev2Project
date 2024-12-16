using System;
using System.Collections.Generic;

namespace AppDev2Project.Models;

public partial class Question
{
    public int Id { get; set; }

    public int ExamId { get; set; }

    public string QuestionText { get; set; } = null!;

    public string QuestionType { get; set; } = null!; // Ensure question_type is either 'multiple_choice' or 'short_answer'

    public string? ChoiceA { get; set; }

    public string? ChoiceB { get; set; }

    public string? ChoiceC { get; set; }

    public string? ChoiceD { get; set; }

    public string CorrectAnswer { get; set; } = null!;

    public double ScoreWeight { get; set; }

    public int? Order { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual ICollection<QuestionAttempt> QuestionAttempt  { get; set; } = new List<QuestionAttempt>(); 
}
