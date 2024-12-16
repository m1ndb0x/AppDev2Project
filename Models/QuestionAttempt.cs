using System;
using System.Collections.Generic;

namespace AppDev2Project.Models;

public partial class QuestionAttempt
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public int UserId { get; set; }

    public string? AnswerText { get; set; }

    public bool IsGraded { get; set; } = false;

    public double? Grade { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.Now;

    public virtual Question Question { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
