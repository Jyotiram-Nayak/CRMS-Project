using Azure;
using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO;
using CRMS_Project.Core.DTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CRMS_Project.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.University)]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        private object response;
        public StudentController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }
        [HttpGet("get-student")]
        public async Task<IActionResult> GetAllStudents([FromQuery]string? userId)
        {
            var result = await _studentRepository.GetStudentAsync(userId);
            if (result == null)
            {
                response = new { success = false, message = "Failed to Fetchd Students.", data = result };
                return BadRequest(response);
            };
            response = new { success = true, message = "Students fetched successfully...", data = result };
            return Ok(response);
        }
        [HttpPost("add-student")]
        public async Task<IActionResult> AddStudent(StudentRequest studentRequest)
        {
            var result = await _studentRepository.AddStudent(studentRequest);
            if (!result.Succeeded)
            {
                response = new { success = false, message = "Failed to Add Student.", data = result };
                return BadRequest(response);
            };
            response = new { success = true, message = "Register Student successfully...", data = result };
            return Ok(response);
        }
        [HttpDelete("delete-student")]
        public async Task<IActionResult> DeleteStudent([FromQuery]string userId)
        {
            var result = await _studentRepository.DeleteStudent(userId);
            if (!result.Succeeded)
            {
                response = new { success = false, message = "Failed to Delete Student.", error = result };
                return BadRequest(response);
            };
            response = new { success = true, message = "Student deleted successfully...", data = result };
            return Ok(response);
        }

    }
}
