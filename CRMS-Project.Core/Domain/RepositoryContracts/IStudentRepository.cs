using CRMS_Project.Core.DTO.Request;
using CRMS_Project.Core.DTO.Response;
using Microsoft.AspNetCore.Identity;

namespace CRMS_Project.Core.Domain.RepositoryContracts
{
    public interface IStudentRepository
    {
        //Task<List<StudentResponse>> GetAllStudentsAsync();
        Task<List<StudentResponse>> GetStudentAsync(string? userId);
        Task<IdentityResult> AddStudent(StudentRequest studentRequest);
    }
}