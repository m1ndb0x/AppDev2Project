using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppDev2Project.Models
{
    public class ExamProgress
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [ForeignKey("Exam")]
        public int ExamId { get; set; }
        
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        
        public DateTime StartedAt { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string SavedAnswers { get; set; }
        public bool IsCompleted { get; set; }
        
        public virtual Exam Exam { get; set; }
        public virtual User User { get; set; }
        public bool IsActive { get; set; } = true;  // Change from internal set to public
    }
}
