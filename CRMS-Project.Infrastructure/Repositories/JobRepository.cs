﻿using AutoMapper;
using CRMS_Project.Core.Domain.Entities;
using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO;
using CRMS_Project.Core.DTO.Request;
using CRMS_Project.Core.DTO.Response;
using CRMS_Project.Core.Enums;
using CRMS_Project.Core.ServiceContracts;
using CRMS_Project.Infrastructure.DbContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRMS_Project.Infrastructure.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobRepository(AppDbContext context,
            IUserService userService,
            IWebHostEnvironment webHostEnvironment,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userService = userService;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<int> CreateJobAsync(JobPostingRequest jobPosting)
        {
            var companyId = _userService.GetUserId();
            var courses = jobPosting.Courses.ToList();
            var job = new JobPosting
            {
                JobId = Guid.NewGuid(),
                CompanyId = companyId,
                UniversityId = jobPosting.UniversityId,
                Courses = courses,
                Title = jobPosting.Title,
                Description = jobPosting.Description,
                PostedDate = DateTime.Now,
                Deadline = jobPosting.Deadline,
                Document = jobPosting.Document,
                CreateOn = DateTime.Now,
                Status = ApplicationStatus.Pending,
            };
            _context.JobPostings.Add(job);
            return await _context.SaveChangesAsync();
        }
        public async Task<List<JobPostingResponse>> GetAllJobsAsync()
        {
            var userRole = _userService.GetUserRole();
            var userId = _userService.GetUserId();
            var applications = await (from JobPostings in _context.JobPostings
                                      join user in _userManager.Users
                                      on (userRole == UserRoles.University ? JobPostings.CompanyId : JobPostings.UniversityId) equals user.Id
                                      where (userRole == UserRoles.University ? JobPostings.UniversityId : JobPostings.CompanyId) == userId
                                      orderby JobPostings.PostedDate descending
                                      select new JobPostingResponse
                                      {
                                          JobId = JobPostings.JobId,
                                          CompanyId = JobPostings.CompanyId,
                                          UniversityId = JobPostings.UniversityId,
                                          Courses = JobPostings.Courses,
                                          Title = JobPostings.Title,
                                          Description = JobPostings.Description,
                                          Status = JobPostings.Status,
                                          PostedDate = JobPostings.PostedDate,
                                          ApprovedDate = JobPostings.ApprovedDate,
                                          RejectedDate = JobPostings.RejectedDate,
                                          Deadline = JobPostings.Deadline,
                                          Document = JobPostings.Document,
                                          FirstName = user.FirstName,
                                          LastName = user.LastName,
                                          Email = user.Email ?? "",
                                          PhoneNumber = user.PhoneNumber ?? "",
                                          City = user.City,
                                          State = user.State,
                                          Website = user.Website,
                                          Bio = user.Bio,
                                          Address = user.Address,
                                      }).ToListAsync();
            return applications;
        }
        public async Task<JobPostingResponse> GetJobByIdAsync(Guid jobId)
        {
            var jobPlacement = await _context.JobPostings.Where(x => x.CompanyId == _userService.GetUserId() && x.JobId == jobId).Select(j => new JobPostingResponse
            {
                JobId = j.JobId,
                CompanyId = j.CompanyId,
                UniversityId = j.UniversityId,
                Title = j.Title,
                Description = j.Description,
                PostedDate = j.PostedDate,
                Deadline = j.Deadline,
                Document = j.Document,
                CreateOn = j.CreateOn,
                UpdateOn = j.UpdateOn,
            }).FirstOrDefaultAsync();
            return jobPlacement;
        }
        public async Task<int> DeleteJobAsync(Guid jobId)
        {
            var jobPlacement = await _context.JobPostings.Where(x => x.CompanyId == _userService.GetUserId() && x.JobId == jobId).FirstOrDefaultAsync();
            if (jobPlacement == null) return 0;
            _context.JobPostings.Remove(jobPlacement);
            return await _context.SaveChangesAsync();
        }
        public async Task<int> UpdateJobAsync(Guid jobId, JobPostingRequest jobPosting)
        {
            var jobPlacement = await _context.JobPostings.Where(x => x.CompanyId == _userService.GetUserId() && x.JobId == jobId).FirstOrDefaultAsync();
            if (jobPlacement == null) { return 0; }
            jobPlacement.UniversityId = jobPosting.UniversityId;
            jobPlacement.Title = jobPosting.Title;
            jobPlacement.Description = jobPosting.Description;
            jobPlacement.Deadline = jobPosting.Deadline;
            jobPlacement.Document = jobPosting.Document;
            jobPlacement.UpdateOn = DateTime.Now;
            return await _context.SaveChangesAsync();
        }
        public async Task<bool> ApproveOrRejectApplicationAsync(Guid jobId, ApplicationStatus status)
        {
            try
            {
                var application = await _context.JobPostings.FindAsync(jobId);
                var universituId = _userService.GetUserId();
                if (application == null && universituId != application?.UniversityId)
                {
                    return false;
                }
                application.Status = status;
                //if(status  == ApplicationStatus.Approved)
                //    application.ApprovedDate = DateTime.Now;
                if (status == ApplicationStatus.Approved)
                {
                    application.ApprovedDate = DateTime.Now;
                }
                else if (status == ApplicationStatus.Rejected)
                {
                    application.RejectedDate = DateTime.Now;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<JobPostingResponse>> GetAllApprovedJobByUniversityId(Guid universityId)
        {
            var userId = _userService.GetUserId().ToString();
            var loginUser = await _userManager.FindByIdAsync(userId);
            var course = loginUser.Course ?? StudentCourse.MCA;
            var applications = await (from JobPostings in _context.JobPostings
                                      join user in _userManager.Users
                                      on JobPostings.CompanyId equals user.Id
                                      where JobPostings.UniversityId == universityId
                                      && JobPostings.Status == ApplicationStatus.Approved
                                      && JobPostings.Courses.Contains(course)

                                      orderby JobPostings.PostedDate descending
                                      select new JobPostingResponse
                                      {
                                          JobId = JobPostings.JobId,
                                          CompanyId = JobPostings.CompanyId,
                                          UniversityId = JobPostings.UniversityId,
                                          Courses = JobPostings.Courses,
                                          Title = JobPostings.Title,
                                          Description = JobPostings.Description,
                                          Status = JobPostings.Status,
                                          PostedDate = JobPostings.PostedDate,
                                          ApprovedDate = JobPostings.ApprovedDate,
                                          RejectedDate = JobPostings.RejectedDate,
                                          Deadline = JobPostings.Deadline,
                                          Document = JobPostings.Document,
                                          FirstName = user.FirstName,
                                          LastName = user.LastName,
                                          Email = user.Email ?? "",
                                          PhoneNumber = user.PhoneNumber ?? "",
                                          City = user.City,
                                          State = user.State,
                                          Website = user.Website,
                                          Bio = user.Bio,
                                          Address = user.Address,
                                      }).ToListAsync();
            return applications;
        }
    }
}
