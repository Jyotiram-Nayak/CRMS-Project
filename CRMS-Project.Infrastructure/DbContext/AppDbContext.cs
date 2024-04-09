using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CRMS_Project.Core.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRMS_Project.Infrastructure.DbContext
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<University> Universities { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Student> Students { get; set; }
        //public DbSet<Job> Jobs { get; set; }
        //public DbSet<JobApplication> JobApplications { get; set; }
        //public DbSet<Message> Messages { get; set; }
        //public DbSet<Resume> Resumes { get; set; }
        public DbSet<UniversityCompany> UniversityCompanies { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UniversityCompany>()
                .HasKey(uc => new { uc.UniversityID, uc.CompanyID });

            modelBuilder.Entity<UniversityCompany>()
                .HasOne(uc => uc.University)
                .WithMany(u => u.UniversityCompanies)
                .HasForeignKey(uc => uc.UniversityID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UniversityCompany>()
                .HasOne(uc => uc.Company)
                .WithMany(c => c.UniversityCompanies)
                .HasForeignKey(uc => uc.CompanyID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
