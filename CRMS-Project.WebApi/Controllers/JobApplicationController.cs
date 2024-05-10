using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO.Request;
using CRMS_Project.Core.DTO;
using CRMS_Project.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRMS_Project.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationController : ControllerBase
    {
        private readonly IJobApplicationRepository _jobApplicationRepository;

        public JobApplicationController(IJobApplicationRepository jobApplicationRepository)
        {
            _jobApplicationRepository = jobApplicationRepository;
        }
        [HttpPost("add-job-application")]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> AddJob(JobApplicationRequest jobApplication)
        {
            (int result,string errorMessage) = await _jobApplicationRepository.AddJobApplicationAsync(jobApplication);
            if (result == 0)
            {
                return BadRequest(new { success = false, message = errorMessage, data = result });
            };
            return Ok(new { success = true, message = "Job apply successfully...", data = result });
        }
        [HttpGet("get-job-applications")]
        [Authorize(Roles = UserRoles.Student+","+ UserRoles.Company+","+ UserRoles.University)]
        public async Task<IActionResult> GetAllJobs()
        {
            var result = await _jobApplicationRepository.GetAllJobApplicationAsync();
            if (result == null)
            {
                return BadRequest(new { success = false, message = "Failed to fetch job application.", data = result });
            };
            return Ok(new { success = true, message = "Job application fetch successfully...", data = result });
        }
        [HttpPut("job-assessment/{assessmentId}")]
        [Authorize(Roles =UserRoles.Company)]
        public async Task<IActionResult> JobAssessment(Guid assessmentId,[FromForm]JobAssessmentRequest jobAssessment)
        {
            var result = await _jobApplicationRepository.JobAssessessmentAsync(assessmentId,jobAssessment);
            if (result == 0)
            {
                return BadRequest(new { success = false, message = "Failed to update job application.", data = result });
            };
            return Ok(new { success = true, message = "Job application fetch successfully...", data = result });
        }
    }
}
