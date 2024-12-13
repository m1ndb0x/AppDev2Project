using Microsoft.AspNetCore.Identity;

namespace AppDev2Project.Models
{
    public class AppUser : IdentityUser
    {
        public string Role { get; set; } // Additional custom fields as we need
        public string? ExamProgress { get; set; }
        
        public DateTime? CreatedAt { get; set; } 
        public virtual ICollection<CompletedExam> CompletedExams { get; set; } = new List<CompletedExam>();
        public virtual ICollection<ExamAttempt> ExamAttempts { get; set; } = new List<ExamAttempt>();
        public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
    }
}

//alex:
// AppUser is going to extend the default IdentityUser class to include custom app-specific properties as needed.
// This is because we need to redo the schema to match the expected name for the user table 'AspNetUsers'
// ASPNET ID will then auto-genereate class `IdentityUser`... god knows why they have different names.
// `IdentityUser` will be used for all authentication and user management once the migration iscomplete.