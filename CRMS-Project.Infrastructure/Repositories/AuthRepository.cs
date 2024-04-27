using AutoMapper;
using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO;
using CRMS_Project.Core.DTO.Request;
using CRMS_Project.Core.DTO.Response;
using CRMS_Project.Core.ServiceContracts;
using CRMS_Project.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRMS_Project.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AuthRepository(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            IUserService userService,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _userService = userService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IdentityResult> RegisterUserAsync(RegisterRequest user)
        {
            ApplicationUser newUser = new ApplicationUser
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsApproved = false,
                Image = user.Image,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                City = user.City,
                State = user.State,
                Website = user.Website,
                Bio = user.Bio,
                Email = user.Email,
                UserName = user.Email,
                CreateOn = DateTime.Now,
                Role = user.Role.ToLower(),
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
        public async Task<IdentityResult> ConfirmEmail(Guid uid, string token)
        {
            token = token.Replace(" ", "+");
            var result = await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(uid.ToString()), token);
            return result;
        }
        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordRequest changePassword)
        {
            var uid = _userService.GetUserId();
            var user = await _userManager.FindByIdAsync(uid.ToString());
            if (user == null)
            {
                return null;
            }
            return await _userManager.ChangePasswordAsync(user, changePassword.CurrentPassword, changePassword.NewPassword);
        }
        public async Task<AuthenticationResponse> GetUserByIdAsunc(Guid? userId)
        {
            if (userId == null)
            {
                userId = _userService.GetUserId();
            }
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) { return null; }
            var userdetails = new AuthenticationResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                Address = user.Address,
                City = user.City,
                State = user.State,
                Website = user.Website,
                Bio = user.Bio,
                Image = user.Image,
                Role = user.Role,
                IsApproved = user.IsApproved,
                CreateOn = user.CreateOn,
                UpdateOn = user.UpdateOn,
            };

            return userdetails;
        }
        public async Task<List<AuthenticationResponse>> GetAllUserByRole(string role)
        {
            var user = await _userManager.Users.Where(x => x.Role == role).Select(x => new AuthenticationResponse
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email ?? "",
                PhoneNumber = x.PhoneNumber ?? "",
                Address = x.Address,
                City = x.City,
                State = x.State,
                Website = x.Website,
                Bio = x.Bio,
                Image = x.Image,
                Role = x.Role,
                IsApproved = x.IsApproved,
                CreateOn = x.CreateOn,
                UpdateOn = x.UpdateOn,
            }).ToListAsync();

            return user;
        }
        public async Task<string> GetUserRole(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with Email '{userEmail}'.");
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null || roles.Count == 0)
            {
                throw new ApplicationException($"No roles found for user with Email '{userEmail}'.");
            }
            // Assuming the user has only one role for simplicity
            return roles.FirstOrDefault();
        }
    }
}
