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
    public virtual ICollection<QuestionAttempt> QuestionAttempts { get; set; } = new List<QuestionAttempt>();
    public string? AssignedStudentIds { get; set; }

    // New properties for status tracking
    public bool IsSubmitted { get; set; }
    public bool IsClosed { get; set; }
    public DateTime? ClosedAt { get; set; }

    // Methods for statistics and analysis
    public double GetClassAverage()
    {
        if (!CompletedExams.Any()) return 0;
        return CompletedExams.Average(ce => ce.TotalScore);
    }
    
    public double GetMedianScore()
    {
        var scores = CompletedExams.Select(ce => ce.TotalScore).OrderBy(s => s).ToList();
        if (!scores.Any()) return 0;
        int mid = scores.Count / 2;
        return scores.Count % 2 != 0 ? scores[mid] : (scores[mid - 1] + scores[mid]) / 2;
    }
    
    public int GetSubmissionCount()
    {
        return CompletedExams.Count(ce => ce.IsSubmitted);
    }
}
