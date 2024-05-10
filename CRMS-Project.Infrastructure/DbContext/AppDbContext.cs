using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CRMS_Project.Core.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CRMS_Project.Core.Domain.Entities;

namespace CRMS_Project.Infrastructure.DbContext
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Student> Students { get; set; }
        //public DbSet<PlacementApplication> PlacementApplications { get; set; }
        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>()
            .HasOne(s => s.ApplicationUser)
            .WithOne()
            .HasForeignKey<Student>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobApplication>()
               .HasOne(ja => ja.JobPosting)
               .WithMany()
               .HasForeignKey(ja => ja.JobId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.Student)
                .WithMany()
                .HasForeignKey(ja => ja.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.Company)
                .WithMany()
                .HasForeignKey(ja => ja.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.University)
                .WithMany()
                .HasForeignKey(ja => ja.UniversityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobPosting>()
                .HasOne(ja => ja.Company)
                .WithMany()
                .HasForeignKey(ja => ja.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobPosting>()
                .HasOne(ja => ja.University)
                .WithMany()
                .HasForeignKey(ja => ja.UniversityId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<PlacementApplication>()
            //    .HasOne(ja => ja.Company)
            //    .WithMany()
            //    .HasForeignKey(ja => ja.CompanyId)
            //    .OnDelete(DeleteBehavior.Restrict);
            
            //modelBuilder.Entity<PlacementApplication>()
            //    .HasOne(ja => ja.University)
            //    .WithMany()
            //    .HasForeignKey(ja => ja.UniversityId)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
