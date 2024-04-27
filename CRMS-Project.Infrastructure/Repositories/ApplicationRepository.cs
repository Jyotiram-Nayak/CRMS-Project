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
using static System.Net.Mime.MediaTypeNames;

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
        public async Task<List<ApplicationResponse>> GetAllApplicationsAsync()
        {
            var userRole = _userService.GetUserRole();
            var userId = _userService.GetUserId();
            var applications = await (from placementApplication in _context.PlacementApplications
                                      join user in _userManager.Users
                                      on (userRole == UserRoles.University ? placementApplication.CompanyId : placementApplication.UniversityId) equals user.Id
                                      where (userRole == UserRoles.University ? placementApplication.UniversityId : placementApplication.CompanyId) == userId
                                      select new ApplicationResponse
                                      {
                                          Id = placementApplication.Id,
                                          CompanyId = placementApplication.CompanyId,
                                          UniversityId = placementApplication.UniversityId,
                                          Status = placementApplication.Status,
                                          DateSubmitted = placementApplication.DateSubmitted,
                                          FirstName = user.FirstName,
                                          LastName = user.LastName,
                                          Email = user.Email ?? "",
                                          PhoneNumber = user.PhoneNumber ?? "",
                                          City = user.City,
                                          State = user.State,
                                          Website = user.Website,
                                          Bio = user.Bio,
                                          Address = user.Address,
                                      }).ToListAsync();
            return applications;
        }
        public async Task<ApplicationResponse> GetApplicationByIdAsync(Guid id)
        {
            var userRole = _userService.GetUserRole();
            var userId = _userService.GetUserId();
            var applications = await (from placementApplication in _context.PlacementApplications
                                      join user in _userManager.Users
                                      on (userRole == UserRoles.University ? placementApplication.CompanyId : placementApplication.UniversityId) equals user.Id
                                      where (userRole == UserRoles.University ? placementApplication.UniversityId : placementApplication.CompanyId) == userId 
                                      && placementApplication.Id == id
                                      select new ApplicationResponse
                                      {
                                          Id = placementApplication.Id,
                                          CompanyId = placementApplication.CompanyId,
                                          UniversityId = placementApplication.UniversityId,
                                          Status = placementApplication.Status,
                                          DateSubmitted = placementApplication.DateSubmitted,
                                          FirstName = user.FirstName,
                                          LastName = user.LastName,
                                          Email = user.Email ?? "",
                                          PhoneNumber = user.PhoneNumber ?? "",
                                          City = user.City,
                                          State = user.State,
                                          Website = user.Website,
                                          Bio = user.Bio,
                                          Address = user.Address,
                                      }).FirstOrDefaultAsync();
            return applications;
        }
        public async Task<int> AddApplication(Guid universityId)
        {
            var university = await _userManager.FindByIdAsync(universityId.ToString());
            if (university == null)
            {
                return 0;
            }
            var application = new PlacementApplication
            {
                Id = Guid.NewGuid(),
                UniversityId = university.Id,
                CompanyId = _userService.GetUserId(),
                Status = ApplicationStatus.Pending,
                DateSubmitted = DateTime.Now

            };
            _context.PlacementApplications.Add(application);
            var result = await _context.SaveChangesAsync();
            return result;
        }
        public async Task<int> DeleteApplicationAsync(Guid id)
        {
            var application = await _context.PlacementApplications.FindAsync(id);
            if(application == null) { return 0; }
            _context.PlacementApplications.Remove(application);
            var result = await _context.SaveChangesAsync();
            return result;
        }
        public async Task<bool> ApproveOrRejectApplicationAsync(Guid applicationId, ApplicationStatus status)
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
