using System;
using System.Collections.Generic;

namespace AppDev2Project.Models;

public partial class CompletedExam
{
    public int Id { get; set; }

    public int? ExamId { get; set; }

    public int? UserId { get; set; }

    public string? ExamStatus { get; set; }

    public double? TotalGrade { get; set; }

    public DateTime? GradedAt { get; set; }

    public bool? NotificationSent { get; set; }

    public virtual Exam? Exam { get; set; }

    public virtual User? User { get; set; }
}
