using Azure;
using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO;
using CRMS_Project.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CRMS_Project.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationRepository _applicationRepository;
        private object response;
        public ApplicationController(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }
        [HttpGet("get-all-application")]
        public async Task<IActionResult> GetApplications()
        {
            var result = _applicationRepository.GetAllApplications();
            if (result == null)
            {
                response = new { success = false, message = "Failed to add Application." };
                return BadRequest(response);
            };
            response = new { success = true, message = "Application added successfully...", data = result };
            return Ok(response);
        }
        [HttpPost("add-application/{universityId}")]
        [Authorize(Roles = UserRoles.Company)]
        public async Task<IActionResult> AddApplication([FromRoute]string universityId)
        {
            var result = await _applicationRepository.AddApplication(universityId);
            if (result == 0)
            {
                response = new { success = false, message = "Failed to add Application."};
                return BadRequest(response);
            };
            response = new { success = true, message = "Application added successfully..."};
            return Ok(response);
        }
        [HttpPut("approve/{applicationId}")]
        [Authorize(Roles = UserRoles.University)]
        public async Task<IActionResult> ApprovePlacementApplication(string applicationId)
        {
            bool result = await _applicationRepository.ApproveOrRejectApplicationAsync(applicationId, ApplicationStatus.Approved);
            if (!result)
            {
                response = new { success = false, message = "Failed to approved application." };
                return BadRequest(response);
            };
            response = new { success = true, message = "Application approved successfully." };
            return Ok(response);
        }
        [HttpPut("reject/{applicationId}")]
        [Authorize(Roles = UserRoles.University)]
        public async Task<IActionResult> RejectPlacementApplication(string applicationId)
        {
            bool result = await _applicationRepository.ApproveOrRejectApplicationAsync(applicationId, ApplicationStatus.Rejected);
            if (!result)
            {
                response = new { success = false, message = "Failed to reject application." };
                return BadRequest(response);
            };
            response = new { success = true, message = "Application rejected successfully." };
            return Ok(response);
        }
    }
}
