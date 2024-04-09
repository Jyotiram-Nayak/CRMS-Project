using Azure;
using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRMS_Project.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private object response;
        public AccountController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromForm]RegisterRequest registerRequest)
        {
            var result =await _authRepository.RegisterUserAsync(registerRequest);
            if (!result.Succeeded)
            {
                response = new { success = false, message = "Failed to Register.", data = result };
                return BadRequest(response);
            };
            response = new { success = true, message = "Register User successfully...", data = result };
            return Ok(response);
        }

    }
}
