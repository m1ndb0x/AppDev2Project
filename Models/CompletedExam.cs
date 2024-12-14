using System;
using System.Collections.Generic;

namespace AppDev2Project.Models;

public partial class CompletedExam
{
    public int Id { get; set; }

    public int ExamId { get; set; }

    public int UserId { get; set; }

    public bool IsCompleted { get; set; } = false;

    public double? TotalScore { get; set; }

    public DateTime? GradedAt { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
