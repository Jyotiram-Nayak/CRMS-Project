using CRMS_Project.Core.DTO.Response;
using CRMS_Project.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.Domain.RepositoryContracts
{
    public interface IApplicationRepository
    {
        Task<List<ApplicationResponse>> GetAllApplications();
        Task<int> AddApplication(string universityId);
        Task<bool> ApproveOrRejectApplicationAsync(string applicationId, ApplicationStatus status);
    }
}
