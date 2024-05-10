using AutoMapper;
using CRMS_Project.Core.Domain.Entities;
using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO;
using CRMS_Project.Core.DTO.Request;
using CRMS_Project.Core.DTO.Response;
using CRMS_Project.Core.Enums;
using CRMS_Project.Core.ServiceContracts;
using CRMS_Project.Infrastructure.DbContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Infrastructure.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobApplicationRepository(AppDbContext context,
            IUserService userService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userService = userService;
            _userManager = userManager;
        }
        public async Task<List<JobApplicationResponse>> GetAllJobApplicationAsync()
        {
            var userRole = _userService.GetUserRole();
            var userId = _userService.GetUserId();

            IQueryable<JobApplication> jobApplicationsQuery = _context.JobApplications
                .Include(x => x.JobPosting)
                .Include(x => x.Student)
                .Include(x => x.Company)
                .Include(x => x.University);

            if (userRole == UserRoles.Student)
            {
                jobApplicationsQuery = jobApplicationsQuery.Where(x => x.StudentId == userId);
            }
            else if (userRole == UserRoles.Company)
            {
                jobApplicationsQuery = jobApplicationsQuery.Where(x => x.CompanyId == userId);
            }
            else if (userRole == UserRoles.University)
            {
                jobApplicationsQuery = jobApplicationsQuery.Where(x => x.UniversityId == userId);
            }
            var jobApplications = await jobApplicationsQuery
                .Select(x => new JobApplicationResponse
                {
                    ApplicationId = x.ApplicationId,
                    JobId = x.JobId,
                    StudentId = userId,
                    CompanyId = x.CompanyId,
                    UniversityId = x.UniversityId,
                    AppliedDate = x.AppliedDate,
                    InterviewDate = x.InterviewDate,
                    isSelected = x.isSelected,
                    Resume = x.Resume,
                    AssessmentLink = x.AssessmentLink,
                    CreateOn = x.CreateOn,
                    UpdateOn = x.UpdateOn,
                    JobTitle = x.JobPosting.Title,
                    StudentName = x.Student.FirstName + " " + x.Student.LastName,
                    StudentEmail = x.Student.Email,
                    CompanyName = x.Company.FirstName + " " + x.Company.LastName,
                    CompanyEmail = x.Company.Email,
                    UniversityName = x.University.FirstName + " " + x.University.LastName,
                    AssessmentCompleted = userRole == UserRoles.Company ? x.AssessmentCompleted : null,
                    AssessmentCompletionDate = userRole == UserRoles.Company ? x.AssessmentCompletionDate : null,
                    AssessmentScore = userRole == UserRoles.Company ? x.AssessmentScore : null,
                    AssessmentFeedback = userRole == UserRoles.Company ? x.AssessmentFeedback : null,
                }).ToListAsync();

            return jobApplications;

        }
        public async Task<(int result, string errorMessage)> AddJobApplicationAsync(JobApplicationRequest jobApplication)
        {
            var userId = _userService.GetUserId();
            var student = _context.Students.Where(x => x.UserId == userId).FirstOrDefault();
            if (student == null) { return (0, "Student details not found."); }
            if (student?.IsSelected == true) { return (0, "Student already selected."); }
            var job = _context.JobPostings.Find(jobApplication.JobId);
            if (job == null) { return (0,"Job not found."); }
            var application = _context.JobApplications.Where(x=>x.StudentId == userId && x.CompanyId == job.CompanyId).FirstOrDefault();
            if(application != null) { return (0, "Already applyed."); }
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) { return (0, "user not found."); }
            if (user.UniversityId != job.UniversityId) { return (0,"unauthorized"); }
            var newJobApplication = new JobApplication
            {
                ApplicationId = Guid.NewGuid(),
                JobId = jobApplication.JobId,
                StudentId = userId,
                CompanyId = job.CompanyId,
                UniversityId = job.UniversityId,
                AppliedDate = DateTime.Now,
                isSelected = SelectionStatus.Pending,
                Resume = jobApplication.Resume,
                CreateOn = DateTime.Now
            };
            _context.JobApplications.Add(newJobApplication);
            var result = await _context.SaveChangesAsync();
            return (result, result > 0 ? "" : "Error occurred while saving the job application.");
        }
        public async Task<int> JobAssessessmentAsync(Guid applicationId, JobAssessmentRequest jobAssessment)
        {
            var userId = _userService.GetUserId();
            var application = await _context.JobApplications.FindAsync(applicationId);
            if (application == null) { return 0; }
            if (application.CompanyId != userId) { return 0; }
            _context.Entry(application).State = EntityState.Modified;
            application.InterviewDate = jobAssessment.InterviewDate;
            application.AssessmentLink = jobAssessment.AssessmentLink;
            application.AssessmentScore = jobAssessment.AssessmentScore;
            application.AssessmentFeedback = jobAssessment.AssessmentFeedback;
            application.AssessmentCompleted = jobAssessment.AssessmentCompleted;
            if (jobAssessment.AssessmentCompleted == true)
            {
                application.AssessmentCompletionDate = DateTime.Now;
            }
            application.isSelected = (SelectionStatus)jobAssessment.isSelected;
            if (application.isSelected == SelectionStatus.Selected)
            {
                var student = _context.Students.Where(x => x.UserId == application.StudentId).FirstOrDefault();
                if (student != null)
                {
                    student.IsSelected = true;
                }
            }
            var result = await _context.SaveChangesAsync();
            return result;
        }
    }
}
