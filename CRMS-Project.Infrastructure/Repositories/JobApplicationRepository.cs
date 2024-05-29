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
        public async Task<List<JobApplicationResponse>> GetAllJobApplicationAsync(PaginationParameters parameters)
        {
            try
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
                var query = jobApplicationsQuery
                    .Select(x => new JobApplicationResponse
                    {
                        ApplicationId = x.ApplicationId,
                        JobId = x.JobId,
                        StudentId = x.StudentId,
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
                        Course = x.Student.Course,
                        City = x.Student.City,
                        State = x.Student.State,
                        CompanyName = x.Company.FirstName + " " + x.Company.LastName,
                        CompanyEmail = x.Company.Email,
                        UniversityName = x.University.FirstName + " " + x.University.LastName,
                        AssessmentCompleted = userRole == UserRoles.Company ? x.AssessmentCompleted : null,
                        AssessmentCompletionDate = userRole == UserRoles.Company ? x.AssessmentCompletionDate : null,
                        AssessmentScore = userRole == UserRoles.Company ? x.AssessmentScore : null,
                        AssessmentFeedback = userRole == UserRoles.Company ? x.AssessmentFeedback : null,
                    });
                if (!string.IsNullOrEmpty(parameters.FilterOn) && !string.IsNullOrEmpty(parameters.FilterQuery))
                {
                    var filterOn = parameters.FilterOn.Trim().ToLowerInvariant();
                    var filterQuery = parameters.FilterQuery.Trim();
                    switch (filterOn)
                    {
                        case "studentname":
                            query = query.Where(application => application.StudentName.Contains(filterQuery));
                            break;
                        case "email":
                            query = query.Where(application => application.StudentEmail.Contains(filterQuery));
                            break;
                        case "course":
                            if (Enum.TryParse(filterQuery, out StudentCourse course))
                            {
                                query = query.Where(application => application.Course == course);
                            }
                            break;
                        case "city":
                            query = query.Where(application => application.City.Contains(filterQuery));
                            break;
                        case "state":
                            query = query.Where(application => application.State.Contains(filterQuery));
                            break;
                        case "isselected":
                            if (Enum.TryParse(filterQuery, out SelectionStatus status))
                            {
                                query = query.Where(application => application.isSelected == status);
                            }
                            break;
                        default:
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(parameters.SortBy))
                {
                    switch (parameters.SortBy.ToLower())
                    {
                        case "studentname":
                            query = parameters.IsAscending ? query.OrderBy(application => application.StudentName) : query.OrderByDescending(application => application.StudentName);
                            break;
                        case "applieddate":
                            query = parameters.IsAscending ? query.OrderBy(application => application.AppliedDate) : query.OrderByDescending(application => application.AppliedDate);
                            break;
                        case "interviewdate":
                            query = parameters.IsAscending ? query.OrderBy(application => application.InterviewDate) : query.OrderByDescending(application => application.InterviewDate);
                            break;
                        case "completiondate":
                            query = parameters.IsAscending ? query.OrderBy(application => application.AssessmentCompletionDate) : query.OrderByDescending(application => application.AssessmentCompletionDate);
                            break;
                        default:
                            query = query.OrderByDescending(application => application.CreateOn);
                            break;
                    }
                }
                else
                {
                    query = query.OrderByDescending(application => application.CreateOn);
                }
                var paginatedQuery = query.Skip((parameters.Page - 1) * parameters.PageSize)
                                                 .Take(parameters.PageSize);
                var jobApplications = await paginatedQuery.ToListAsync();
                return jobApplications;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<(int result, string errorMessage)> AddJobApplicationAsync(JobApplicationRequest jobApplication)
        {
            try
            {
                var userId = _userService.GetUserId();
                var student = await _context.Students.Where(x => x.UserId == userId).FirstOrDefaultAsync();
                if (student == null) { return (0, "Student details not found."); }
                if (student?.IsSelected == true) { return (0, "Student already selected."); }
                var job = _context.JobPostings.Find(jobApplication.JobId);
                if (job == null) { return (0, "Job not found."); }
                var application = _context.JobApplications.Where(x => x.StudentId == userId && x.CompanyId == job.CompanyId).FirstOrDefault();
                if (application != null) { return (0, "Already applyed."); }
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null) { return (0, "user not found."); }
                if (user.UniversityId != job.UniversityId) { return (0, "unauthorized"); }
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
            catch (Exception)
            {
                return (0, "Error occurred while saving the job application.");
            }
        }
        public async Task<(int result, string errorMessage)> JobAssessessmentAsync(Guid applicationId, JobAssessmentRequest jobAssessment)
        {
            try
            {
                var userId = _userService.GetUserId();
                var application = await _context.JobApplications.FindAsync(applicationId);
                if (application == null) { return (0, "Application not found"); }
                if (application.CompanyId != userId) { return (0, "Invalid request"); }
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
                application.isSelected = jobAssessment.isSelected.HasValue ?
                    (SelectionStatus)jobAssessment.isSelected : SelectionStatus.Pending;
                if (application.isSelected == SelectionStatus.Selected)
                {
                    var student = _context.Students.Where(x => x.UserId == application.StudentId).FirstOrDefault();
                    if (student != null)
                    {
                        student.IsSelected = true;
                    }
                }
                var result = await _context.SaveChangesAsync();
                return (result, result > 0 ? "" : "Error occurred while updating the job application.");
            }
            catch (Exception)
            {
                return (0, "Error occurred while updating the job application.");
            }
        }
    }
}
