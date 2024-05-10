using Azure;
using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO;
using CRMS_Project.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace CRMS_Project.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationRepository _applicationRepository;
        public ApplicationController(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }
        [HttpGet("get-all-applications")]
        [Authorize(Roles = UserRoles.Company + "," + UserRoles.University)]
        public async Task<IActionResult> GetApplications()
        {
            var result = _applicationRepository.GetAllApplicationsAsync();
            if (result == null)
            {
                return BadRequest(new { success = false, message = "Failed to fetch applications." });
            };
            return Ok(new { success = true, message = "Applications fetched successfully...", data = result });
        }
        [HttpGet("get-application-details/{id}")]
        [Authorize(Roles = UserRoles.Company + "," + UserRoles.University)]
        public async Task<IActionResult> GetApplicationDetails(Guid id)
        {
            var result = _applicationRepository.GetApplicationByIdAsync(id);
            if (result == null)
            {
                return BadRequest(new { success = false, message = "Failed to fetch application." });
            };
            return Ok(new { success = true, message = "Application fetched successfully...", data = result });
        }
        [HttpPost("add-application/{universityId}")]
        [Authorize(Roles = UserRoles.Company)]
        public async Task<IActionResult> AddApplication([FromRoute] Guid universityId)
        {
            var result = await _applicationRepository.AddApplication(universityId);
            if (result == 0)
            {
                return BadRequest(new { success = false, message = "Failed to add Application." });
            };
            if (result == 1)
            {
                return BadRequest(new { success = false, message = "Already applied." });
            };
            return Ok(new { success = true, message = "Application added successfully..." });
        }
        [HttpDelete("cancel-application/{id}")]
        [Authorize(Roles = UserRoles.Company)]
        public async Task<IActionResult> CancelApplication([FromRoute] Guid id)
        {
            var result = await _applicationRepository.DeleteApplicationAsync(id);
            if (result == 0)
            {
                return BadRequest(new { success = false, message = "Failed to cancel Application." });
            };
            return Ok(new { success = true, message = "Application canceled successfully..." });
        }
        [HttpPut("approve/{applicationId}")]
        [Authorize(Roles = UserRoles.University)]
        public async Task<IActionResult> ApprovePlacementApplication(Guid applicationId)
        {
            bool result = await _applicationRepository.ApproveOrRejectApplicationAsync(applicationId, ApplicationStatus.Approved);
            if (!result)
            {
                return BadRequest(new { success = false, message = "Failed to approve application." });
            };
            return Ok(new { success = true, message = "Application approved successfully." });
        }
        [HttpPut("reject/{applicationId}")]
        [Authorize(Roles = UserRoles.University)]
        public async Task<IActionResult> RejectPlacementApplication(Guid applicationId)
        {
            bool result = await _applicationRepository.ApproveOrRejectApplicationAsync(applicationId, ApplicationStatus.Rejected);
            if (!result)
            {
                return BadRequest(new { success = false, message = "Failed to reject application." });
            };
            return Ok(new { success = true, message = "Application rejected successfully." });
        }
        [HttpGet("get-approved-universitys")]
        [Authorize(Roles = UserRoles.Company)]
        public async Task<IActionResult> GetAllApprovedUniversity(Guid applicationId)
        {
            var result = await _applicationRepository.GetAllApprovedUniversity();
            if (result == null)
            {
                return BadRequest(new { success = false, message = "Failed to fetched university."});
            };
            return Ok(new { success = true, message = "University fetched successfully.", data = result });
        }
    }
}
