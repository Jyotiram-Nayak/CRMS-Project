using AutoMapper;
using CRMS_Project.Core.Domain.Entities;
using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO.Request;
using CRMS_Project.Core.DTO.Response;
using CRMS_Project.Core.ServiceContracts;
using CRMS_Project.Infrastructure.DbContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace CRMS_Project.Infrastructure.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public JobRepository(AppDbContext context,
            IUserService userService,
            IWebHostEnvironment webHostEnvironment,
            IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        public async Task<int> CreateJobAsync(JobPostingRequest jobPosting)
        {
            var companyId = _userService.GetUserId();
            var placement = await _context.PlacementApplications.Where(x => x.CompanyId == companyId && x.UniversityId == jobPosting.UniversityId).FirstOrDefaultAsync();
            if (placement == null) { return 0; }
            string DocPath = "";
            if (jobPosting.Document != null)
            {
                DocPath = "Jobpdfs/";
                DocPath += Guid.NewGuid().ToString() + "_" + jobPosting.Document.FileName.Replace(" ","-");
                string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, DocPath);
                await jobPosting.Document.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
            }
            var job = new JobPosting
            {
                JobId = Guid.NewGuid(),
                CompanyId = _userService.GetUserId(),
                UniversityId = jobPosting.UniversityId,
                Title = jobPosting.Title,
                Description = jobPosting.Description,
                PostedDate = DateTime.Now,
                Deadline = jobPosting.Deadline,
                Document = DocPath,
                CreateOn = DateTime.Now,
            };
            _context.JobPostings.Add(job);
            return await _context.SaveChangesAsync();
        }
        public async Task<List<JobPostingResponse>> GetAllJobsAsync()
        {
            var jobPlacement = await _context.JobPostings.Where(j => j.CompanyId == _userService.GetUserId()).Select(j => new JobPostingResponse
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
            }).ToListAsync();
            return jobPlacement;
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
            string DocPath = "";
            if (jobPosting.Document != null)
            {
                DocPath = "Jobpdfs/";
                DocPath += Guid.NewGuid().ToString() + "_" + jobPosting.Document.FileName;
                string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, DocPath);
                await jobPosting.Document.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
            }
            var jobPlacement = await _context.JobPostings.Where(x => x.CompanyId == _userService.GetUserId() && x.JobId == jobId).FirstOrDefaultAsync();
            if (jobPosting == null) { return 0; }
            jobPlacement.UniversityId = jobPosting.UniversityId;
            jobPlacement.Title = jobPosting.Title;
            jobPlacement.Description = jobPosting.Description;
            jobPlacement.Deadline = jobPosting.Deadline;
            jobPlacement.Document = DocPath;
            jobPlacement.UpdateOn = DateTime.Now;
            return await _context.SaveChangesAsync();
        }
    }
}
