using System;
using System.Collections.Generic;

namespace AppDev2Project.Models;

public partial class Question
{
    public int Id { get; set; }

    public int? ExamId { get; set; }

    public string QuestionText { get; set; } = null!;

    public string QuestionType { get; set; } = null!;

    public string? ChoiceA { get; set; }

    public string? ChoiceB { get; set; }

    public string? ChoiceC { get; set; }

    public string? ChoiceD { get; set; }

    public string? CorrectChoiceAnswer { get; set; }

    public string? CorrectTextAnswer { get; set; }

    public double? ScoreWeight { get; set; }

    public int? Order { get; set; }

    public virtual Exam? Exam { get; set; }

    public virtual ICollection<ExamAttempt> ExamAttempts { get; set; } = new List<ExamAttempt>();
}
