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
    [PersonalData]
    public string Name { get; set; } = null!;  // This is the display name that can contain spaces

    // [Required]
    // [StringLength(50)]
    public string Role { get; set; } = "Student"; // Default role

    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    public virtual ICollection<CompletedExam> CompletedExams { get; set; } = new List<CompletedExam>();

    public virtual ICollection<QuestionAttempt> QuestionAttempt { get; set; } = new List<QuestionAttempt>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public string ProfilePictureUrl { get; set; } = string.Empty;

    // Property to get user's initials
    public string Initials
    {
        get
        {
            if (!string.IsNullOrEmpty(Name))
            {
                var initials = string.Join("", Name.Split(' ')
                                    .Where(w => !string.IsNullOrEmpty(w))
                                    .Select(w => w[0]))
                                    .ToUpper();
                return initials.Length > 2 ? initials.Substring(0, 2) : initials;
            }
            return "NA"; // Default fallback if Name is empty
        }
    }

}
