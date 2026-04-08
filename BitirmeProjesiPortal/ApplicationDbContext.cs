using BitirmeProjesiPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace BitirmeProjesiPortal.Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<UserAccount>? UserAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserAccountClassReference>()
                .HasOne(uacr => uacr.User)
                .WithMany()
                .HasForeignKey(uacr => uacr.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserAccountClassReference>()
                .HasOne(uacr => uacr.ClassReference)
                .WithMany()
                .HasForeignKey(uacr => uacr.ClassReferenceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<Class>? Classes { get; set; }
        public DbSet<ClassReference>? ClassReferences { get; set; }
        public DbSet<Announcement>? Announcements { get; set; }
        public DbSet<Assignment>? Assignments { get; set; }
        public DbSet<UserAccountClassReference>? UserAccountClassReferences { get; set; }
        public DbSet<ClassFile>? ClassFiles { get; set; }
        public DbSet<Grade>? Grades { get; set; }
        public DbSet<ExamType>? ExamTypes { get; set; }

    }
}
