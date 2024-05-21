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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CRMS_Project.Infrastructure.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobRepository(AppDbContext context,
            IUserService userService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userService = userService;
            _userManager = userManager;
        }

        public async Task<int> CreateJobAsync(JobPostingRequest jobPosting)
        {
            try
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
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<List<JobPostingResponse>> GetAllJobsAsync(PaginationParameters parameters)
        {
            try
            {
                var userRole = _userService.GetUserRole();
                var userId = _userService.GetUserId();
                var query = from JobPostings in _context.JobPostings
                            join user in _userManager.Users
                            on (userRole == UserRoles.University ? JobPostings.CompanyId : JobPostings.UniversityId) equals user.Id
                            where (userRole == UserRoles.University ? JobPostings.UniversityId : JobPostings.CompanyId) == userId
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
                                Image = user.Image,
                            };
                if (!string.IsNullOrEmpty(parameters.FilterOn) && !string.IsNullOrEmpty(parameters.FilterQuery))
                {
                    switch (parameters.FilterOn.ToLower())
                    {
                        case "firstname":
                            query = query.Where(job => job.FirstName.Contains(parameters.FilterQuery));
                            break;
                        case "email":
                            query = query.Where(job => job.Email.Contains(parameters.FilterQuery));
                            break;
                        case "title":
                            query = query.Where(job => job.Title.Contains(parameters.FilterQuery));
                            break;
                        case "city":
                            query = query.Where(job => job.City.Contains(parameters.FilterQuery));
                            break;
                        case "state":
                            query = query.Where(job => job.State.Contains(parameters.FilterQuery));
                            break;
                        default:
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(parameters.SortBy))
                {
                    switch (parameters.SortBy.ToLower())
                    {
                        case "posteddate":
                            query = parameters.IsAscending ? query.OrderBy(job => job.PostedDate) : query.OrderByDescending(job => job.PostedDate);
                            break;
                        case "title":
                            query = parameters.IsAscending ? query.OrderBy(job => job.Title) : query.OrderByDescending(job => job.Title);
                            break;
                        case "firstname":
                            query = parameters.IsAscending ? query.OrderBy(job => job.FirstName) : query.OrderByDescending(job => job.FirstName);
                            break;
                        case "city":
                            query = parameters.IsAscending ? query.OrderBy(job => job.City) : query.OrderByDescending(job => job.City);
                            break;
                        case "state":
                            query = parameters.IsAscending ? query.OrderBy(job => job.State) : query.OrderByDescending(job => job.State);
                            break;
                        default:
                            query = query.OrderByDescending(job => job.CreateOn);
                            break;
                    }
                }
                else
                {
                    query = query.OrderByDescending(job => job.PostedDate);
                }
                var paginatedQuery = query.Skip((parameters.Page - 1) * parameters.PageSize)
                                                 .Take(parameters.PageSize);

                var jobPostings = await paginatedQuery.ToListAsync();
                return jobPostings;

            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<JobPostingResponse> GetJobByIdAsync(Guid jobId)
        {
            try
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
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<(int result, string errorMessage)> DeleteJobAsync(Guid jobId)
        {
            try
            {
                var jobPlacement = await _context.JobPostings.Where(x => x.CompanyId == _userService.GetUserId() && x.JobId == jobId).FirstOrDefaultAsync();
                if (jobPlacement == null) return (0, "Invalid request");
                _context.JobPostings.Remove(jobPlacement);
                var result = await _context.SaveChangesAsync();
                return (result, result > 0 ? "" : "Error occurred while deleting the job.");
            }
            catch (Exception)
            {
                return (0, "Error occurred while deleting the job.");
            }
        }
        public async Task<(int result, string errorMessage)> UpdateJobAsync(Guid jobId, JobPostingRequest jobPosting)
        {
            try
            {
                var jobPlacement = await _context.JobPostings.Where(x => x.CompanyId == _userService.GetUserId() && x.JobId == jobId).FirstOrDefaultAsync();
                if (jobPlacement == null) { return (0, "Invalid request"); }
                jobPlacement.UniversityId = jobPosting.UniversityId;
                jobPlacement.Title = jobPosting.Title;
                jobPlacement.Description = jobPosting.Description;
                jobPlacement.Deadline = jobPosting.Deadline;
                jobPlacement.Document = jobPosting.Document;
                jobPlacement.UpdateOn = DateTime.Now;
                var result = await _context.SaveChangesAsync();
                return (result, result > 0 ? "" : "Error occurred while updating the job.");
            }
            catch (Exception)
            {
                return (0, "Error occurred while updating the job.");
            }
        }
        public async Task<(int result, string errorMessage)> ApproveOrRejectApplicationAsync(Guid jobId, ApplicationStatus status)
        {
            try
            {
                var application = await _context.JobPostings.FindAsync(jobId);
                var universituId = _userService.GetUserId();
                if (application == null && universituId != application?.UniversityId)
                {
                    return (0, "Invalid request");
                }
                application.Status = status;
                if (status == ApplicationStatus.Approved)
                {
                    application.ApprovedDate = DateTime.Now;
                }
                else if (status == ApplicationStatus.Rejected)
                {
                    application.RejectedDate = DateTime.Now;
                }
                var result = await _context.SaveChangesAsync();
                return (result, result > 0 ? "" : "Error occurred while saving the job application.");
            }
            catch (Exception)
            {
                return (0, "Error occurred while saving the job application.");
            }
        }

        public async Task<List<JobPostingResponse>> GetAllApprovedJobByUniversityId(Guid universityId, PaginationParameters parameters)
        {
            try
            {
                var userId = _userService.GetUserId().ToString();
                var loginUser = await _userManager.FindByIdAsync(userId);
                var course = loginUser?.Course ?? StudentCourse.MCA;
                var query = from JobPostings in _context.JobPostings
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
                                Image = user.Image,
                            };

                if (!string.IsNullOrEmpty(parameters.FilterOn) && !string.IsNullOrEmpty(parameters.FilterQuery))
                {
                    switch (parameters.FilterOn.ToLower())
                    {
                        case "firstname":
                            query = query.Where(job => job.FirstName.Contains(parameters.FilterQuery));
                            break;
                        case "email":
                            query = query.Where(job => job.Email.Contains(parameters.FilterQuery));
                            break;
                        case "title":
                            query = query.Where(job => job.Title.Contains(parameters.FilterQuery));
                            break;
                        case "city":
                            query = query.Where(job => job.City.Contains(parameters.FilterQuery));
                            break;
                        case "state":
                            query = query.Where(job => job.State.Contains(parameters.FilterQuery));
                            break;
                        default:
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(parameters.SortBy))
                {
                    switch (parameters.SortBy.ToLower())
                    {
                        case "posteddate":
                            query = parameters.IsAscending ? query.OrderBy(job => job.PostedDate) : query.OrderByDescending(job => job.PostedDate);
                            break;
                        case "title":
                            query = parameters.IsAscending ? query.OrderBy(job => job.Title) : query.OrderByDescending(job => job.Title);
                            break;
                        case "firstname":
                            query = parameters.IsAscending ? query.OrderBy(job => job.FirstName) : query.OrderByDescending(job => job.FirstName);
                            break;
                        case "city":
                            query = parameters.IsAscending ? query.OrderBy(job => job.City) : query.OrderByDescending(job => job.City);
                            break;
                        case "state":
                            query = parameters.IsAscending ? query.OrderBy(job => job.State) : query.OrderByDescending(job => job.State);
                            break;
                        default:
                            query = query.OrderByDescending(job => job.CreateOn);
                            break;
                    }
                }
                else
                {
                    query = query.OrderByDescending(job => job.PostedDate);
                }
                var paginatedQuery = query.Skip((parameters.Page - 1) * parameters.PageSize)
                                                 .Take(parameters.PageSize);

                var applications = await paginatedQuery.ToListAsync();
                return applications;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
