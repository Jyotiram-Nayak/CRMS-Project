using Azure;
using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO.Request;
using CRMS_Project.Core.ServiceContracts;
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
        private readonly IJwtService _jwtService;
        private object response;
        public AccountController(IAuthRepository authRepository,
            IJwtService jwtService)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody]RegisterRequest registerRequest)
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
        [HttpPost("login-user")]
        public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest)
        {
            var result = await _authRepository.LoginAsync(loginRequest);
            if (!result.Succeeded)
            {
                return Unauthorized(new { success = false, message = "SignIn failed." });
            }
            var token = await _jwtService.GenerateJWTTokenAsync(loginRequest.Email);
            response = new { success = true, message = "SignIn successfully.", data = new {token } };
            return Ok(response);
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string uid, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(token))
            {
                return BadRequest();
            }
            //token = token.Replace(" ", "+");
            var result = await _authRepository.ConfirmEmail(uid, token);
            if (!result.Succeeded)
            {
                return Unauthorized();
            }
            return Ok("Thank you for varification");
        }
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePassword)
        {
            var result = await _authRepository.ChangePasswordAsync(changePassword);
            if (!result.Succeeded)
            {
                return Unauthorized();
            }
            return Ok(result);
        }
    }
}
