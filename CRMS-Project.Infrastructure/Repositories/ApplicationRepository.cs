using CRMS_Project.Core.Domain.Entities;
using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO;
using CRMS_Project.Core.DTO.Response;
using CRMS_Project.Core.Enums;
using CRMS_Project.Core.ServiceContracts;
using CRMS_Project.Infrastructure.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Infrastructure.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public ApplicationRepository(AppDbContext context
            , UserManager<ApplicationUser> userManager
            , IUserService userService)
        {
            _context = context;
            _userManager = userManager;
            _userService = userService;
        }
        public async Task<List<ApplicationResponse>> GetAllApplications()
        {
            var userRole = _userService.GetUserRole();
            var userId = _userService.GetUserId();

            var query = _context.PlacementApplications.AsQueryable();

            if (userRole == UserRoles.University)
            {
                query = query.Where(a => a.UniversityId == userId);
            }
            else if (userRole == UserRoles.Company)
            {
                query = query.Where(a => a.CompanyId == userId);
            }

            var applications = await query
                .Select(a => new ApplicationResponse
                {
                    Id = a.Id,
                    CompanyId = a.CompanyId,
                    UniversityId = a.UniversityId,
                    Status = a.Status,
                    DateSubmitted = a.DateSubmitted,
                    // Map additional details based on user role
                    //FirstName = a.ApplicationUser.FirstName,
                    //LastName = a.User.LastName,
                    //Email = a.User.Email,
                    //Address = a.User.Address,
                    //Website = a.User.Website,
                    //Image = a.User.Image,
                    //Role = a.User.Role,
                    //IsApproved = a.User.IsApproved,
                    //CreateOn = a.User.CreateOn,
                    //UpdateOn = a.User.UpdateOn
                    // Map other properties as needed
                })
                .ToListAsync();

            return applications;
        }
        public async Task<int> AddApplication(string universityId)
        {
            var university = await _userManager.FindByIdAsync(universityId);
            if (university == null)
            {
                return 0;
            }
            var application = new PlacementApplication
            {
                Id = Guid.NewGuid().ToString(),
                UniversityId = university.Id,
                CompanyId = _userService.GetUserId(),
                Status = ApplicationStatus.Pending,
                DateSubmitted = DateTime.Now
            };
            _context.PlacementApplications.Add(application);
            var result = await _context.SaveChangesAsync();
            return result;
        }
        public async Task<bool> ApproveOrRejectApplicationAsync(string applicationId, ApplicationStatus status)
        {
            try
            {
                var application = await _context.PlacementApplications.FindAsync(applicationId);
                var universituId = _userService.GetUserId();
                if (application == null && universituId != application?.UniversityId)
                {
                    return false;
                }
                application.Status = status;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
