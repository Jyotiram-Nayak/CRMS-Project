﻿using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.DTO.Request;
using CRMS_Project.Core.DTO.Response;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.Domain.RepositoryContracts
{
    public interface IAuthRepository
    {
        Task<IdentityResult> RegisterUserAsync(RegisterRequest registerRequest);
        Task<SignInResult> LoginAsync(LoginRequest loginRequest);
        Task<IdentityResult> ConfirmEmail(Guid uid, string token);
        Task<IdentityResult> ChangePasswordAsync(ChangePasswordRequest changePassword);
        Task<AuthenticationResponse> GetUserByIdAsunc(Guid? userId);
        Task<List<AuthenticationResponse>> GetAllUserByRole(string role,PaginationParameters parameters);
        Task<IdentityResult> ApproveUserAsync(Guid userId);
        Task<IdentityResult> UpdateUserAsync(UpdateUserRequest updateUser);
        Task<IdentityResult> SendForgotPasswordEmail(string email);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordRequest resetPassword);
        Task<UniversityDashboardResponse> UniversityDashboard();
        Task<CompanyDashboardResponse> CompanyDashboard();
        Task<StudentDashboardResponse> StudentDashboard();
        Task<AdminDashboardResponse> AdminDashboard();
        Task<IdentityResult> SendContactusMail(ContactUsRequest contact);
    }
}
