using System;
using System.Collections.Generic;

namespace AppDev2Project.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<CompletedExam> CompletedExams { get; set; } = new List<CompletedExam>();

    public virtual ICollection<ExamAttempt> ExamAttempts { get; set; } = new List<ExamAttempt>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
