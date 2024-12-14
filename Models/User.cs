using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

/* alex:
Since ASPNET ID is going to auto-gen the IdentityUser class, 
this is now just a temporary placeholder until the migration to ASP.NET Identity (via AppUser) is complete. 
Once the migration is successful, AppUser will be used for our user management, 
(because ASPNET ID is a precious about what goes into the `AspNetUser` table, but AppUser says "please" i guess...)
and the old `User` model will become obselete.
Savvy?
*/

namespace AppDev2Project.Models;

public partial class User : IdentityUser<int> // Derive from IdentityUser<int>
{
    public string Name { get; set; } = null!;

    public string Role { get; set; } = null!; // Ensure role is either 'student' or 'teacher'

    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    public virtual ICollection<CompletedExam> CompletedExams { get; set; } = new List<CompletedExam>();

    public virtual ICollection<ExamAttempt> ExamAttempts { get; set; } = new List<ExamAttempt>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
