using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO;
using CRMS_Project.Core.DTO.Request;
using CRMS_Project.Core.DTO.Response;
using CRMS_Project.Core.ServiceContracts;
using CRMS_Project.Core.Services;
using Microsoft.AspNetCore.Identity;

namespace CRMS_Project.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public AuthRepository(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _userService = userService;
        }
        public async Task<IdentityResult> RegisterUserAsync(RegisterRequest user)
        {
            ApplicationUser newUser = new ApplicationUser
            {
                FirstName=user.FirstName,
                LastName=user.LastName,
                IsApproved = false,
                Image = user.Image,
                Address=user.Address,
                Website=user.Website,
                Email = user.Email,
                UserName = user.Email,
                CreateOn = DateTime.Now,
                Role=user.Role.ToLower(),
            };
            IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);
            if (!result.Succeeded)
            {
                return result;
            }
            switch (user.Role.ToLower())
            {
                case "university":
                    await _userManager.AddToRoleAsync(newUser, UserRoles.University);
                    break;
                case "company":
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Company);
                    break;
                case "student":
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Student);
                    break;
                default:
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Student);
                    break;
            }
            await _emailService.SendEmailConfirmationAsync(newUser);
            return result;
        }

        public async Task<SignInResult> LoginAsync(LoginRequest loginRequest)
        {
            return await _signInManager.PasswordSignInAsync(loginRequest.Email, loginRequest.Password, false, false);
        }
        public async Task<IdentityResult> ConfirmEmail(string uid, string token)
        {
            token = token.Replace(" ", "+");
            var result = await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(uid), token);
            return result;
        }
        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordRequest changePassword)
        {
            var uid = _userService.GetUserId();
            var user = await _userManager.FindByIdAsync(uid);
            if (user == null)
            {
                return null;
            }
            return await _userManager.ChangePasswordAsync(user, changePassword.CurrentPassword, changePassword.NewPassword);
        }
        public async Task<AuthenticationResponse> GetUserDetails(string? userId)
        {
            if (userId == null)
            {
                userId = _userService.GetUserId();
            }
            var user = await _userManager.FindByIdAsync(userId);
            return new AuthenticationResponse();
        }
        //public async Task<AuthenticationResponse> GetAllUser(UserRoles userRoles)
        //{
            
        //}
    }
}
