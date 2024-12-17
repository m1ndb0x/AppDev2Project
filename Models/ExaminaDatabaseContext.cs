using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppDev2Project.Models
{
    public class ExaminaDatabaseContext : IdentityDbContext<User, IdentityRole<int>, int> // Use IdentityDbContext
    {
        public ExaminaDatabaseContext(DbContextOptions<ExaminaDatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<Exam> Exams { get; set; } = null!;
        public DbSet<Question> Questions { get; set; } = null!;
        public DbSet<QuestionAttempt> QuestionAttempt { get; set; } = null!;
        public DbSet<CompletedExam> CompletedExams { get; set; } = null!;
        // Remove ExamStudents DbSet

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255); // Use PasswordHash from IdentityUser
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.ToTable("exams");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).HasColumnType("TEXT");
                entity.Property(e => e.Subject).HasMaxLength(255);
                entity.Property(e => e.State).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalScoreWeight).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.AvailableFrom).IsRequired();
                entity.Property(e => e.AvailableUntil);
                entity.HasOne(e => e.Teacher)
                      .WithMany(u => u.CreatedExams)
                      .HasForeignKey(e => e.TeacherId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.AssignedStudents)
                    .WithMany(u => u.AssignedExams)
                    .UsingEntity(j => j.ToTable("exam_student_assignments"));
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("questions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.QuestionText).IsRequired().HasColumnType("TEXT");
                entity.Property(e => e.QuestionType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CorrectAnswer).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ScoreWeight).IsRequired();
                entity.Property(e => e.Order);
                entity.HasOne(e => e.Exam)
                      .WithMany(ex => ex.Questions)
                      .HasForeignKey(e => e.ExamId)
                      .OnDelete(DeleteBehavior.Cascade); // Changed from Restrict to Cascade
            });

            modelBuilder.Entity<QuestionAttempt>(entity =>
            {
                entity.ToTable("exam_attempt");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AnswerText).HasColumnType("TEXT");
                entity.Property(e => e.IsGraded).HasDefaultValue(false);
                entity.Property(e => e.Grade);
                entity.Property(e => e.SubmittedAt).HasDefaultValueSql("GETDATE()");
                entity.HasOne(e => e.Question)
                      .WithMany(q => q.QuestionAttempt)
                      .HasForeignKey(e => e.QuestionId)
                      .OnDelete(DeleteBehavior.Cascade); // Changed from Restrict to Cascade
                entity.HasOne(e => e.User)
                      .WithMany(u => u.QuestionAttempt)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CompletedExam>(entity =>
            {
                entity.ToTable("completed_exams");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IsCompleted).HasDefaultValue(false);
                entity.Property(e => e.TotalScore);
                entity.Property(e => e.CompletedAt).HasColumnType("datetime2").IsRequired();
                entity.Property(e => e.GradedAt).HasColumnType("datetime2");
                entity.HasOne(e => e.Exam)
                      .WithMany(ex => ex.CompletedExams)
                      .HasForeignKey(e => e.ExamId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.User)
                      .WithMany(u => u.CompletedExams)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
