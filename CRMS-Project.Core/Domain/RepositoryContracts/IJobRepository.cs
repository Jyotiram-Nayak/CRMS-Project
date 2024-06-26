﻿using CRMS_Project.Core.Domain.Entities;
using CRMS_Project.Core.DTO.Request;
using CRMS_Project.Core.DTO.Response;
using CRMS_Project.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.Domain.RepositoryContracts
{
    public interface IJobRepository
    {
        Task<int> CreateJobAsync(JobPostingRequest jobPosting);
        Task<JobPostingResponse> GetJobByIdAsync(Guid jobId);
        Task<List<JobPostingResponse>> GetAllJobsAsync(PaginationParameters parameters);
        Task<(int result, string errorMessage)> UpdateJobAsync(Guid jobId, JobPostingRequest jobPosting);
        Task<(int result, string errorMessage)> DeleteJobAsync(Guid jobId);
        Task<(int result, string errorMessage)> ApproveOrRejectApplicationAsync(Guid jobId, ApplicationStatus status);
        Task<List<JobPostingResponse>> GetAllApprovedJobByUniversityId(Guid universityId, PaginationParameters parameters);
    }
}
