using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO;
using CRMS_Project.Core.DTO.Request;
using CRMS_Project.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;

namespace CRMS_Project.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailServices _emailServices;

        public AuthRepository(UserManager<ApplicationUser> userManager,IEmailServices emailServices)
        {
            _userManager = userManager;
            _emailServices = emailServices;
        }
        public async Task<IdentityResult> RegisterUserAsync(RegisterRequest registerRequest)
        {
            ApplicationUser newUser = new ApplicationUser
            {
                Email = registerRequest.Email,
                UserName = registerRequest.Email,
                Image = registerRequest.Image,
                IsApproved = false
            };
            IdentityResult result = await _userManager.CreateAsync(newUser, registerRequest.Password);
            if (!result.Succeeded)
            {
                return result;
            }
            switch (registerRequest.Role)
            {
                case "University":
                    await _userManager.AddToRoleAsync(newUser, UserRoles.University);
                    break;
                case "Company":
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Company);
                    break;
                case "Student":
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Student);
                    break;
                default:
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Student);
                    break;
            }
            await _emailServices.SendEmailConfirmationAsync(newUser);
            return result;
        }
    }
}
