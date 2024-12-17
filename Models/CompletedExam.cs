using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppDev2Project.Models;

public partial class CompletedExam
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ExamId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public double TotalScore { get; set; }

    [Required]
    public DateTime CompletedAt { get; set; }

    public DateTime? GradedAt { get; set; }

    // Navigation properties
    [ForeignKey("ExamId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Exam Exam { get; set; }

    [ForeignKey("UserId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual User User { get; set; }

    [Required]
    public bool IsCompleted { get; set; }
}
