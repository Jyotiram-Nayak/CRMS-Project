using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO;
using CRMS_Project.Core.DTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRMS_Project.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Company)]
    public class JobController : ControllerBase
    {
        private readonly IJobRepository _jobRepository;

        public JobController(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }
        [HttpGet("get-all-job")]
        public async Task<IActionResult> GetAllJobPosting()
        {
            var result = await _jobRepository.GetAllJobsAsync();
            if (result == null)
            {
                return BadRequest(new { success = false, message = "Failed to fetch Job.", data = result });
            };
            return Ok(new { success = true, message = "Job fetched successfully...", data = result });
        }
        [HttpGet("get-job-details/{jobid}")]
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
        public async Task<IActionResult> DeleteJob(Guid jobId)
        {
            var result = await _jobRepository.DeleteJobAsync(jobId);
            if (result == 0)
            {
                return BadRequest(new { success = false, message = "Failed to delete Job.", data = result });
            };
            return Ok(new { success = true, message = "Job deleted successfully...", data = result });
        }

        [HttpPut("update-job/{jobId}")]
        public async Task<IActionResult> UpdateJob([FromRoute]Guid jobId,JobPostingRequest jobPosting)
        {
            var result = await _jobRepository.UpdateJobAsync(jobId, jobPosting);
            if (result == 0)
            {
                return BadRequest(new { success = false, message = "Failed to update Job.", data = result });
            };
            return Ok(new { success = true, message = "Job updated successfully...", data = result });
        }
    }
}
