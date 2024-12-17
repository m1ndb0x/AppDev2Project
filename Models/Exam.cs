using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppDev2Project.Models;

public partial class Exam
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    [StringLength(255)]
    public string Subject { get; set; } = null!;

    [Required]
    [Range(0, 100)]
    public double TotalScoreWeight { get; set; }

    [Required]
    public string State { get; set; } = "Incomplete";

    [Required]
    public int TeacherId { get; set; }

    [ForeignKey("TeacherId")]
    public virtual User? Teacher { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    [Required]
    [Range(1, 480)]  // 1 minute to 8 hours
    public int Duration { get; set; } = 60;  // Default duration of 60 minutes (1 hour)

    // Add new properties for tracking
    public bool HasStarted { get; set; } = false;
    public DateTime? StartedAt { get; set; }
    public virtual ICollection<ExamProgress> StudentProgress { get; set; } = new List<ExamProgress>();

    public virtual ICollection<User> AssignedStudents { get; set; } = new List<User>();
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    public virtual ICollection<CompletedExam> CompletedExams { get; set; } = new List<CompletedExam>();
    public virtual ICollection<QuestionAttempt> QuestionAttempt { get; set; } = new List<QuestionAttempt>();
    public string? AssignedStudentIds { get; set; }
}
