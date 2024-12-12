using Microsoft.EntityFrameworkCore;

public class ExaminaDbContext : DbContext
{
public ExaminaDbContext(DbContextOptions<ExaminaDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Exam> Exam { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<ExamAttempt> ExamAttempts { get; set; }
    public DbSet<CompletedExam> CompletedExams { get; set; }
}