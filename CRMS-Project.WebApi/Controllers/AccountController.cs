using Azure;
using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO;
using CRMS_Project.Core.DTO.Request;
using CRMS_Project.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(IAuthRepository authRepository,
            IJwtService jwtService,
            UserManager<ApplicationUser> userManager)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
            _userManager = userManager;
        }
        [HttpGet("get-user-profile")]
        [Authorize]
        public async Task<IActionResult> GetUserById()
        {
            var result = await _authRepository.GetUserByIdAsunc(null);
            if (result == null)
            {
                return BadRequest(new { success = false, message = "User not found." });
            }
            return Ok(new { success = true, message = "User details fetched successfully.", data = result });
        }
        [HttpGet("get-user-details/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(Guid? userId)
        {
            var result = await _authRepository.GetUserByIdAsunc(userId);
            if (result == null)
            {
                return BadRequest(new { success = false, message = "User not found." });
            }
            return Ok(new { success = true, message = "User details fetched successfully.", data = result });
        }
        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody]RegisterRequest registerRequest)
        {
            var result =await _authRepository.RegisterUserAsync(registerRequest);
            if (!result.Succeeded)
            {
                return BadRequest(new { success = false, message = "Failed to register user.", errors = result.Errors });
            };
            return Ok(new { success = true, message = "Register User successfully...", data = result });
        }
        [HttpPost("login-user")]
        public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest)
        {
            var result = await _authRepository.LoginAsync(loginRequest);
            if (!result.Succeeded)
            {
                return BadRequest(new { success = false, message = "Sign in failed. Invalid email or password." });
            }
            var token = await _jwtService.GenerateJWTTokenAsync(loginRequest.Email);
            var role = await _authRepository.GetUserRole(loginRequest.Email);
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            return Ok(new { success = true, message = "Sign in successful.", data = new { token,role,user} });
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] Guid uid, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(uid.ToString()) || string.IsNullOrEmpty(token))
            {
                return BadRequest();
            }
            var result = await _authRepository.ConfirmEmail(uid, token);
            if (!result.Succeeded)
            {
                return BadRequest(new { success = false, message = "Failed to confirm Email." ,errors = result.Errors });
            }
            return Ok(new { success = true, message = "Email confirmed successfully.", data = result });
        }
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePassword)
        {
            var result = await _authRepository.ChangePasswordAsync(changePassword);
            if (!result.Succeeded)
            {
                return BadRequest(new { success = false, message = "Failed to change password." , errors = result.Errors });
            }
            return Ok(new { success = true, message = "Password changed successfully.", data = result });
        }
        [HttpPut("update-user")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(UpdateUserRequest updateUser)
        {
            var result = await _authRepository.UpdateUserAsync(updateUser);
            if (!result.Succeeded)
            {
                return BadRequest(new { success = false, message = "Failed to update details.", errors = result.Errors });
            }
            return Ok(new { success = true, message = "Profile updated successfully.", data = result });
        }

        [HttpGet("get-all-company")]
        [Authorize]
        public async Task<IActionResult> GetAllCompany()
        {
            var result = await _authRepository.GetAllUserByRole(UserRoles.Company.ToLower());
            if (result == null)
            {
                return BadRequest(new { success = false, message = "User not found." });
            }
            return Ok(new { success = true, message = "User details fetched successfully.", data = result });
        }
        [HttpGet("get-all-university")]
        [Authorize]
        public async Task<IActionResult> GetAllUniversity()
        {
            var result = await _authRepository.GetAllUserByRole(UserRoles.University.ToLower());
            if (result == null)
            {
                return BadRequest(new { success = false, message = "User not found." });
            }
            return Ok(new { success = true, message = "User details fetched successfully.", data = result });
        }
    }
}
