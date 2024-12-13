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

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    /* alex:
    So `UserName` is required by ASP.NET Identity for authentication.
    It's used as unique identifier during login. 
    We COULD (and I did) set `UserName` as the email for login purposes ( view: AccountController.cs ).
    The custom 'Name' property is for storing the user's full name and isn't used for auth.
    */

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<CompletedExam> CompletedExams { get; set; } = new List<CompletedExam>();

    public virtual ICollection<ExamAttempt> ExamAttempts { get; set; } = new List<ExamAttempt>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
