using System;
using System.Collections.Generic;

namespace AppDev2Project.Models;

public partial class ExamAttempt
{
    public int Id { get; set; }

    public int? ExamId { get; set; }

    public int? UserId { get; set; }

    public int? QuestionId { get; set; }

    public string? AnswerText { get; set; }

    public string? FilePath { get; set; }

    public double? Grade { get; set; }

    public string? Status { get; set; }

    public DateTime? AnswerModifiedAt { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public virtual Exam? Exam { get; set; }

    public virtual Question? Question { get; set; }

    public virtual User? User { get; set; }
}
