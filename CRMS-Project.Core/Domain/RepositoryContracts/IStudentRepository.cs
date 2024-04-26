using CRMS_Project.Core.DTO.Request;
using CRMS_Project.Core.DTO.Response;
using Microsoft.AspNetCore.Identity;

namespace CRMS_Project.Core.Domain.RepositoryContracts
{
    public interface IStudentRepository
    {
        Task<List<StudentResponse>> GetAllStudentsAsync();
        Task<StudentResponse> GetStudentByIdAsync(Guid userId);
        Task<IdentityResult> AddStudent(StudentRequest studentRequest);
        Task<IdentityResult> DeleteStudent(Guid id);
        Task<IdentityResult> UpdateStudentAsync(Guid studentId, StudentRequest studentRequest);
    }
}