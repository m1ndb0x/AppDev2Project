using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppDev2Project.Models;

public partial class QuestionAttempt
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int QuestionId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int ExamId { get; set; }

    public string? AnswerText { get; set; }

    public bool IsGraded { get; set; } = false;

    public double? Grade { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.Now;

    [ForeignKey("QuestionId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Question Question { get; set; } = null!;

    [ForeignKey("UserId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual User User { get; set; } = null!;

    [ForeignKey("ExamId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Exam Exam { get; set; } = null!;
}
