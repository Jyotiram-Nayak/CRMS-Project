using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO;
using CRMS_Project.Core.DTO.Request;
using CRMS_Project.Core.Enums;
using CRMS_Project.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRMS_Project.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class JobController : ControllerBase
    {
        private readonly IJobRepository _jobRepository;

        public JobController(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }
        [HttpGet("get-all-job")]
        [Authorize(Roles = UserRoles.Company + "," + UserRoles.University + "," + UserRoles.Admin)]
        public async Task<IActionResult> GetAllJobPosting([FromQuery]PaginationParameters parameters)
        {
            var result = await _jobRepository.GetAllJobsAsync(parameters);
            if (result == null)
            {
                return BadRequest(new { success = false, message = "Failed to fetch Job.", data = result });
            };
            return Ok(new { success = true, message = "Job fetched successfully...", data = result });
        }
        [HttpGet("get-all-jobs-by-university/{universityId}")]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> GetAllApprovedJobByUniversityId(Guid universityId, [FromQuery] PaginationParameters parameters)
        {
            var result = await _jobRepository.GetAllApprovedJobByUniversityId(universityId,parameters);
            if (result == null)
            {
                return BadRequest(new { success = false, message = "Failed to fetch Jobs.", data = result });
            };
            return Ok(new { success = true, message = "Jobs fetched successfully...", data = result });
        }
        [HttpGet("get-job-details/{jobid}")]
        [Authorize(Roles = UserRoles.Company)]
        public async Task<IActionResult> GetJobPosting(Guid jobid)
        {
            var result = await _jobRepository.GetJobByIdAsync(jobid);
            if (result == null)
            {
                return BadRequest(new { success = false, message = "Failed to fetch Job.", data = result });
            };
            return Ok(new { success = true, message = "Job fetched successfully...", data = result });
        }
        [HttpPost("add-job")]
        [Authorize(Roles = UserRoles.Company)]
        public async Task<IActionResult> AddJob(JobPostingRequest jobPosting)
        {
            var result = await _jobRepository.CreateJobAsync(jobPosting);
            if (result == 0)
            {
                return BadRequest(new { success = false, message = "Failed to add Job.", data = result });
            };
            return Ok(new { success = true, message = "Job added successfully...", data = result });
        }
        [HttpDelete("delete-job/{jobId}")]
        [Authorize(Roles = UserRoles.Company)]
        public async Task<IActionResult> DeleteJob(Guid jobId)
        {
            (int result, string errorMessage) = await _jobRepository.DeleteJobAsync(jobId);
            if (result == 0)
            {
                return BadRequest(new { success = false, message = "Failed to delete Job." + errorMessage });
            };
            return Ok(new { success = true, message = "Job deleted successfully...", data = result });
        }

        [HttpPut("update-job/{jobId}")]
        [Authorize(Roles = UserRoles.Company)]
        public async Task<IActionResult> UpdateJob([FromRoute] Guid jobId, JobPostingRequest jobPosting)
        {
            (int result, string errorMessage) = await _jobRepository.UpdateJobAsync(jobId, jobPosting);
            if (result == 0)
            {
                return BadRequest(new { success = false, message = "Failed to update Job." + errorMessage });
            };
            return Ok(new { success = true, message = "Job updated successfully...", data = result });
        }
        [HttpPut("approve/{jobId}")]
        [Authorize(Roles = UserRoles.University)]
        public async Task<IActionResult> ApprovePlacementApplication(Guid jobId)
        {
            (int result, string errorMessage) = await _jobRepository.ApproveOrRejectApplicationAsync(jobId, ApplicationStatus.Approved);
            if (result == 0)
            {
                return BadRequest(new { success = false, message = "Failed to approve application." + errorMessage });
            };
            return Ok(new { success = true, message = "Application approved successfully." });
        }
        [HttpPut("reject/{jobId}")]
        [Authorize(Roles = UserRoles.University)]
        public async Task<IActionResult> RejectPlacementApplication(Guid jobId)
        {
            (int result, string errorMessage) = await _jobRepository.ApproveOrRejectApplicationAsync(jobId, ApplicationStatus.Rejected);
            if (result == 0)
            {
                return BadRequest(new { success = false, message = "Failed to reject application. " + errorMessage });
            };
            return Ok(new { success = true, message = "Application rejected successfully." });
        }


    }
}
