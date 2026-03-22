using BlazorApp8.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp8.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ExamAttempt> ExamAttempts { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<ExamAttempt>()
                .HasOne(ea => ea.Student)
                .WithMany()
                .HasForeignKey(ea => ea.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StudentAnswer>()
                .HasOne(sa => sa.Question)
                .WithMany()
                .HasForeignKey(sa => sa.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
