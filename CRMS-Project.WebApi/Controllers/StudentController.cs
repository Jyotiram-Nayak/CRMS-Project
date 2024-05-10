using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO;
using CRMS_Project.Core.DTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRMS_Project.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.University)]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        public StudentController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }
        [HttpGet("get-all-students")]
        public async Task<IActionResult> GetAllStudents([FromQuery]FilterStudentRequest filterStudent)
        {
            var result = await _studentRepository.GetAllStudentsAsync(filterStudent);
            if (result == null)
            {
                return BadRequest(new { success = false, message = "Failed to Fetchd Students.", data = result });
            };
            return Ok(new { success = true, message = "Students fetched successfully...", data = result });
        }
        [HttpGet("get-student-details/{userId}")]
        public async Task<IActionResult> GetAllStudents([FromRoute] Guid userId)
        {
            var result = await _studentRepository.GetStudentByIdAsync(userId);
            if (result == null)
            {
                return BadRequest(new { success = false, message = "Failed to fetchd Student details.", data = result });
            };
            return Ok(new { success = true, message = "Student details fetched successfully...", data = result });
        }
        [HttpPost("add-student")]
        public async Task<IActionResult> AddStudent([FromForm]StudentRequest studentRequest)
        {
            var result = await _studentRepository.AddStudent(studentRequest);
            if (!result.Succeeded)
            {
                return BadRequest(new { success = false, message = "Failed to Add Student.", data = result });
            };
            return Ok(new { success = true, message = "Register Student successfully...", data = result });
        }
        [HttpPut("update-student/{studentId}")]
        public async Task<IActionResult> UpdateStudent([FromRoute] Guid studentId, [FromForm] UpdateStudentRequest studentRequest)
        {
            var result = await _studentRepository.UpdateStudentAsync(studentId, studentRequest);
            if (!result.Succeeded)
            {
                return BadRequest(new { success = false, message = "Failed to update Student.", data = result });
            };
            return Ok(new { success = true, message = "Student updated successfully...", data = result });
        }
        [HttpDelete("delete-student/{studentId}")]
        public async Task<IActionResult> DeleteStudent(Guid studentId)
        {
            var result = await _studentRepository.DeleteStudent(studentId);
            if (!result.Succeeded)
            {
                return BadRequest(new { success = false, message = "Failed to Delete Student.", error = result });
            };
            return Ok(new { success = true, message = "Student deleted successfully...", data = result });
        }

    }
}
