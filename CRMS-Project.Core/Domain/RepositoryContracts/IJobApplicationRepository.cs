﻿using CRMS_Project.Core.DTO.Request;
using CRMS_Project.Core.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.Domain.RepositoryContracts
{
    public interface IJobApplicationRepository
    {
        Task<List<JobApplicationResponse>> GetAllJobApplicationAsync(PaginationParameters parameters);
        Task<(int result, string errorMessage)> AddJobApplicationAsync(JobApplicationRequest jobApplication);
        Task<(int result, string errorMessage)> JobAssessessmentAsync(Guid applicationId, JobAssessmentRequest jobAssessment);
    }
}
