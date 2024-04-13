﻿using CRMS_Project.Core.DTO.Response;
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
        Task<List<ApplicationResponse>> GetAllApplicationsAsync();
        Task<ApplicationResponse> GetAllApplicationByIdAsync(Guid id);
        Task<int> AddApplication(Guid universityId);
        Task<bool> ApproveOrRejectApplicationAsync(Guid applicationId, ApplicationStatus status);
    }
}
