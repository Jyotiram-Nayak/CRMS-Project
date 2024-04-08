using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Infrastructure.DbContext
{
    internal class AppDbContext:IdentityDbContext
    {
        //public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        //{

        //}
        //public DbSet<University> Universities { get; set; }
        //public DbSet<Company> Companies { get; set; }
        //public DbSet<Student> Students { get; set; }
        ////public DbSet<Job> Jobs { get; set; }
        ////public DbSet<JobApplication> JobApplications { get; set; }
        ////public DbSet<Message> Messages { get; set; }
        ////public DbSet<Resume> Resumes { get; set; }
        //public DbSet<UniversityCompany> UniversityCompanies { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<UniversityCompany>()
        //        .HasKey(uc => new { uc.UniversityID, uc.CompanyID });

        //    modelBuilder.Entity<UniversityCompany>()
        //        .HasOne(uc => uc.University)
        //        .WithMany(u => u.UniversityCompanies)
        //        .HasForeignKey(uc => uc.UniversityID)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    modelBuilder.Entity<UniversityCompany>()
        //        .HasOne(uc => uc.Company)
        //        .WithMany(c => c.UniversityCompanies)
        //        .HasForeignKey(uc => uc.CompanyID)
        //        .OnDelete(DeleteBehavior.Cascade);
        //}
    }
}
