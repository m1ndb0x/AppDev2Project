using System;
using System.Collections.Generic;

namespace AppDev2Project.Models;

public partial class Exam
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Subject { get; set; }

    public int? TeacherId { get; set; }

    public bool? IsClosed { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<CompletedExam> CompletedExams { get; set; } = new List<CompletedExam>();

    public virtual ICollection<ExamAttempt> ExamAttempts { get; set; } = new List<ExamAttempt>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual User? Teacher { get; set; }
}
