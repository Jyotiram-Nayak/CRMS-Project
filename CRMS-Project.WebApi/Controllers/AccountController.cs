using Azure;
using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO;
using CRMS_Project.Core.DTO.Email;
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
        private readonly IUserService _userService;

        public AccountController(IAuthRepository authRepository,
            IJwtService jwtService,
            UserManager<ApplicationUser> userManager,
            IUserService userService)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
            _userManager = userManager;
            _userService = userService;
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
                var errorMessage = "Failed to register user. Errors: " + string.Join(", ", result.Errors.Select(e => e.Description));
                // Return a BadRequest with the custom error message
                return BadRequest(new { success = false, message = errorMessage });
                //return BadRequest(new { success = false, message = "Failed to register user.", errors = result.Errors });
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
            //var role = await _authRepository.GetUserRole(loginRequest.Email);
            var role = _userService.GetUserRole();
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
                return BadRequest(new { success = false, message = "Failed to change password" + string.Join(", ", result.Errors.Select(e => e.Description))});
            }
            return Ok(new { success = true, message = "Password changed successfully.", data = result });
        }
        [HttpPost("forgot-password/{email}")]
        [Authorize]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var result = await _authRepository.SendForgotPasswordEmail(email);
            if (!result.Succeeded)
            {
                return BadRequest(new { success = false, message = "Failed to forgot password" + string.Join(", ", result.Errors.Select(e => e.Description)) });
            }
            return Ok(new { success = true, message = "Email send successfully.", data = result });
        }
        [HttpPut("reset-password")]
        [Authorize]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPassword)
        {
            var result = await _authRepository.ResetPasswordAsync(resetPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new { success = false, message = "Failed to reset password" + string.Join(", ", result.Errors.Select(e => e.Description)) });
            }
            return Ok(new { success = true, message = "Password reset successfully.", data = result });
        }

        [HttpPut("update-user")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(UpdateUserRequest updateUser)
        {
            var result = await _authRepository.UpdateUserAsync(updateUser);
            if (!result.Succeeded)
            {
                return BadRequest(new { success = false, message = "Failed to update details" + string.Join(", ", result.Errors.Select(e => e.Description)) });
            }
            var user = await _userManager.FindByEmailAsync(updateUser.Email);
            return Ok(new { success = true, message = "Profile updated successfully.", data =new { result,user} });
        }
        [HttpPut("approve-user/{userId}")]
        [Authorize(Roles =UserRoles.Admin)]
        public async Task<IActionResult> ApproveUser(Guid userId)
        {
            var result = await _authRepository.ApproveUserAsync(userId);
            if (!result.Succeeded)
            {
                return BadRequest(new { success = false, message = "Failed to update user" + string.Join(", ", result.Errors.Select(e => e.Description)) });
            }
            return Ok(new { success = true, message = "User updated successfully." });
        }
        [HttpGet("get-all-company")]
        [Authorize]
        public async Task<IActionResult> GetAllCompany([FromQuery]PaginationParameters parameters)
        {
            var result = await _authRepository.GetAllUserByRole(UserRoles.Company.ToLower(),parameters);
            if (result == null)
            {
                return BadRequest(new { success = false, message = "Company not found." });
            }
            return Ok(new { success = true, message = "User details fetched successfully.", data = result });
        }
        [HttpGet("get-all-university")]
        [Authorize]
        public async Task<IActionResult> GetAllUniversity([FromQuery]PaginationParameters parameters)
        {
            var result = await _authRepository.GetAllUserByRole(UserRoles.University.ToLower(),parameters);
            if (result == null)
            {
                return BadRequest(new { success = false, message = "University not found." });
            }
            return Ok(new { success = true, message = "User details fetched successfully.", data = result });
        }
        [HttpGet("university-dashboard")]
        [Authorize(Roles =UserRoles.University)]
        public async Task<IActionResult> GetUniversityDashboard()
        {
            var result = await _authRepository.UniversityDashboard();
            if (result == null)
            {
                return BadRequest(new { success = false, message = "Faild to load Dashboard." });
            }
            return Ok(new { success = true, message = "Dashboard load successfully.", data = result });
        }
        [HttpGet("company-dashboard")]
        [Authorize(Roles = UserRoles.Company)]
        public async Task<IActionResult> GetCompanyDashboard()
        {
            var result = await _authRepository.CompanyDashboard();
            if (result == null)
            {
                return BadRequest(new { success = false, message = "Faild to load Dashboard." });
            }
            return Ok(new { success = true, message = "Dashboard load successfully.", data = result });
        }
        [HttpGet("student-dashboard")]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> GetStudentDashboard()
        {
            var result = await _authRepository.StudentDashboard();
            if (result == null)
            {
                return BadRequest(new { success = false, message = "Faild to load Dashboard." });
            }
            return Ok(new { success = true, message = "Dashboard load successfully.", data = result });
        }
        [HttpGet("admin-dashboard")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var result = await _authRepository.AdminDashboard();
            if (result == null)
            {
                return BadRequest(new { success = false, message = "Faild to load Dashboard." });
            }
            return Ok(new { success = true, message = "Dashboard load successfully.", data = result });
        }
        [HttpPatch("contact-us")]
        public async Task<IActionResult> SendMailContactUs(EmailMessage emailMessage)
        {
            var result = await _authRepository.StudentDashboard();
            if (result == null)
            {
                return BadRequest(new { success = false, message = "Faild to send mail." });
            }
            return Ok(new { success = true, message = "Mail send successfully.", data = result });
        }
    }
}
