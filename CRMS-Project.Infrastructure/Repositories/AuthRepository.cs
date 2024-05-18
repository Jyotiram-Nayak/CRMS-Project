using AutoMapper;
using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO;
using CRMS_Project.Core.DTO.Request;
using CRMS_Project.Core.DTO.Response;
using CRMS_Project.Core.Enums;
using CRMS_Project.Core.ServiceContracts;
using CRMS_Project.Infrastructure.DbContext;
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
        private readonly IStudentRepository _student;
        private readonly AppDbContext _context;

        public AuthRepository(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            IUserService userService,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment,
            IStudentRepository student,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _userService = userService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _student = student;
            _context = context;
        }
        public async Task<IdentityResult> RegisterUserAsync(RegisterRequest user)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser != null) { return IdentityResult.Failed(new IdentityError { Description = "Email is already registered." }); }
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
                    default:
                        await _userManager.AddToRoleAsync(newUser, UserRoles.University);
                        break;
                }
                await _emailService.SendEmailConfirmationAsync(newUser);
                return result;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }

        public async Task<SignInResult> LoginAsync(LoginRequest loginRequest)
        {
            return await _signInManager.PasswordSignInAsync(loginRequest.Email, loginRequest.Password, false, false);
        }
        public async Task<IdentityResult> ConfirmEmail(Guid uid, string token)
        {
            try
            {
                token = token.Replace(" ", "+");
                var user = await _userManager.FindByIdAsync(uid.ToString());
                if (user == null) { return IdentityResult.Failed(new IdentityError { Description = "User not found." }); }
                var result = await _userManager.ConfirmEmailAsync(user, token);
                return result;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }
        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordRequest changePassword)
        {
            try
            {
                var uid = _userService.GetUserId();
                var user = await _userManager.FindByIdAsync(uid.ToString());
                if (user == null) { return IdentityResult.Failed(new IdentityError { Description = "User not found." }); }
                return await _userManager.ChangePasswordAsync(user, changePassword.CurrentPassword, changePassword.NewPassword);
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }
        public async Task<AuthenticationResponse> GetUserByIdAsunc(Guid? userId)
        {
            try
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
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<AuthenticationResponse>> GetAllUserByRole(string role)
        {
            try
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
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<IdentityResult> UpdateUserAsync(UpdateUserRequest updateUser)
        {
            try
            {
                var userId = _userService.GetUserId().ToString();
                // Find the user by userId
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) { IdentityResult.Failed(new IdentityError { Description = $"User with ID '{userId}' not found." }); }
                user.FirstName = updateUser.FirstName;
                user.LastName = updateUser.LastName;
                user.Email = updateUser.Email;
                user.PhoneNumber = updateUser.PhoneNumber;
                user.Address = updateUser.Address;
                user.City = updateUser.City;
                user.State = updateUser.State;
                user.Website = updateUser.Website;
                user.Bio = updateUser.Bio;
                user.Image = updateUser.Image;
                user.UpdateOn = DateTime.Now;
                // Call UserManager's UpdateAsync method to update the user
                return await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }
        public async Task<IdentityResult> SendForgotPasswordEmail(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null) { return IdentityResult.Failed(new IdentityError { Description = "User not found" }); }
                await _emailService.SendForgotEmailAsync(user);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }
        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordRequest resetPassword)
        {
            try
            {
                resetPassword.Token = resetPassword.Token.Replace(" ", "+");
                var user = await _userManager.FindByIdAsync(resetPassword.Uid.ToString());
                if (user == null) { return IdentityResult.Failed(new IdentityError { Description = "Failed to reset password." }); }
                var result = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.NewPassword);
                return result;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }
        public async Task<UniversityDashboardResponse> UniversityDashboard()
        {
            try
            {
                var userId = _userService.GetUserId();
                var totalJobs = await _context.JobPostings.Where(x => x.UniversityId == userId).CountAsync();
                var allStudents = await _userManager.Users.Where(x => x.Role == UserRoles.Student && x.UniversityId == userId).CountAsync();
                var selectedStudentCount = await _context.Students
                                        .Where(s => s.IsSelected == true)
                                        .Join(
                                            _context.Users,
                                            student => student.UserId,
                                            user => user.Id,
                                            (student, user) => new { Student = student, User = user }
                                        )
                                        .Where(joinResult => joinResult.User.UniversityId == userId)
                                        .CountAsync();
                var pendingStudentCount = await _context.Students
                                       .Where(s => s.IsSelected == false)
                                       .Join(
                                           _context.Users,
                                           student => student.UserId,
                                           user => user.Id,
                                           (student, user) => new { Student = student, User = user }
                                       )
                                       .Where(joinResult => joinResult.User.UniversityId == userId)
                                       .CountAsync();
                var response = new UniversityDashboardResponse
                {

                    AllStudents = allStudents,
                    SelectedStudents = selectedStudentCount,
                    PendingStudents = pendingStudentCount,
                    TotalJobs = totalJobs
                };
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<CompanyDashboardResponse> CompanyDashboard()
        {
            try
            {
                var userId = _userService.GetUserId();
                var totalJobs = await _context.JobPostings.Where(x => x.CompanyId == userId).CountAsync();
                var totalApplication = await _context.JobApplications.Where(x => x.CompanyId == userId).CountAsync();
                int pendingStudents = await _context.JobApplications.Where(x => x.CompanyId == userId && x.isSelected == SelectionStatus.Pending).CountAsync();
                var selectedStudents = await _context.JobApplications.Where(x => x.CompanyId == userId && x.isSelected == SelectionStatus.Selected).CountAsync();
                var rejectedStudents = await _context.JobApplications.Where(x => x.CompanyId == userId && x.isSelected == SelectionStatus.Rejected).CountAsync();
                var result = new CompanyDashboardResponse
                {
                    TotalJobs = totalJobs,
                    TotalApplication = totalApplication,
                    SelectedStudents = selectedStudents,
                    PendingStudents = pendingStudents,
                    RejectedStudents = rejectedStudents,
                };
                return result;
            }
            catch (Exception)
            {

                return null;
            }
        }
        public async Task<StudentDashboardResponse> StudentDashboard()
        {
            try
            {
                var userId = _userService.GetUserId();
                var user = await _userManager.FindByIdAsync(userId.ToString());
                var studentCourse = user?.Course;
                var totalJobs = await _context.JobPostings.Where(x => x.UniversityId == user.UniversityId && x.Courses.Any(x => x == studentCourse)).CountAsync();
                var totalApplication = await _context.JobApplications.Where(x => x.StudentId == userId).CountAsync();
                var result = new StudentDashboardResponse
                {
                    TotalJobs = totalJobs,
                    TotalApplication = totalApplication
                };
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
