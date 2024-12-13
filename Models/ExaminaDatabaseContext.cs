using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AppDev2Project.Models
{
    public partial class ExaminaDatabaseContext : IdentityDbContext<IdentityUser>
    {
        public ExaminaDatabaseContext(DbContextOptions<ExaminaDatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CompletedExam> CompletedExams { get; set; }
        public virtual DbSet<Exam> Exams { get; set; }
        public virtual DbSet<ExamAttempt> ExamAttempts { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CompletedExam>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__complete__3213E83FC787E29A");

                entity.ToTable("completed_exams");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ExamId).HasColumnName("exam_id");
                entity.Property(e => e.ExamStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("exam_status");
                entity.Property(e => e.GradedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("graded_at");
                entity.Property(e => e.NotificationSent)
                    .HasDefaultValue(false)
                    .HasColumnName("notification_sent");
                entity.Property(e => e.TotalGrade).HasColumnName("total_grade");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Exam).WithMany(p => p.CompletedExams)
                    .HasForeignKey(d => d.ExamId)
                    .HasConstraintName("FK__completed__exam___6D0D32F4");

                entity.HasOne(d => d.User).WithMany(p => p.CompletedExams)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__completed__user___6E01572D");
            });

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__exams__3213E83FC5F1922D");

                entity.ToTable("exams");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasColumnName("description");
                entity.Property(e => e.IsClosed)
                    .HasDefaultValue(false)
                    .HasColumnName("is_closed");
                entity.Property(e => e.Subject)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("subject");
                entity.Property(e => e.TeacherId).HasColumnName("teacher_id");
                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("title");

                entity.HasOne(d => d.Teacher).WithMany(p => p.Exams)
                    .HasForeignKey(d => d.TeacherId)
                    .HasConstraintName("FK__exams__teacher_i__5FB337D6");
            });

            modelBuilder.Entity<ExamAttempt>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__exam_att__3213E83F6D8E0C4C");

                entity.ToTable("exam_attempt");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AnswerModifiedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("answer_modified_at");
                entity.Property(e => e.AnswerText)
                    .HasColumnType("text")
                    .HasColumnName("answer_text");
                entity.Property(e => e.ExamId).HasColumnName("exam_id");
                entity.Property(e => e.FilePath)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("file_path");
                entity.Property(e => e.Grade).HasColumnName("grade");
                entity.Property(e => e.QuestionId).HasColumnName("question_id");
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("status");
                entity.Property(e => e.SubmittedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("submitted_at");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Exam).WithMany(p => p.ExamAttempts)
                    .HasForeignKey(d => d.ExamId)
                    .HasConstraintName("FK__exam_atte__exam___68487DD7");

                entity.HasOne(d => d.Question).WithMany(p => p.ExamAttempts)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("FK__exam_atte__quest__6A30C649");

                entity.HasOne(d => d.User).WithMany(p => p.ExamAttempts)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__exam_atte__user___693CA210");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__question__3213E83FA8196DFA");

                entity.ToTable("questions");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ChoiceA)
                    .HasColumnType("text")
                    .HasColumnName("choice_a");
                entity.Property(e => e.ChoiceB)
                    .HasColumnType("text")
                    .HasColumnName("choice_b");
                entity.Property(e => e.ChoiceC)
                    .HasColumnType("text")
                    .HasColumnName("choice_c");
                entity.Property(e => e.ChoiceD)
                    .HasColumnType("text")
                    .HasColumnName("choice_d");
                entity.Property(e => e.CorrectChoiceAnswer)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("correct_choice_answer");
                entity.Property(e => e.CorrectTextAnswer)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("correct_text_answer");
                entity.Property(e => e.ExamId).HasColumnName("exam_id");
                entity.Property(e => e.Order).HasColumnName("order");
                entity.Property(e => e.QuestionText)
                    .HasColumnType("text")
                    .HasColumnName("question_text");
                entity.Property(e => e.QuestionType)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("question_type");
                entity.Property(e => e.ScoreWeight)
                    .HasDefaultValue(1.0)
                    .HasColumnName("score_weight");

                entity.HasOne(d => d.Exam).WithMany(p => p.Questions)
                    .HasForeignKey(d => d.ExamId)
                    .HasConstraintName("FK__questions__exam___6477ECF3");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__users__3213E83F9DE08643");

                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("email");
                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name");
                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("password");
                entity.Property(e => e.Role)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("role");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
