using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO;
using CRMS_Project.Core.DTO.Request;
using CRMS_Project.Core.DTO.Response;
using CRMS_Project.Core.ServiceContracts;
using CRMS_Project.Infrastructure.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CRMS_Project.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly AppDbContext _context;
        private readonly IUserService _userService;

        public StudentRepository(UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            AppDbContext context,
            IUserService userService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _context = context;
            _userService = userService;
        }
        public async Task<List<StudentResponse>> GetAllStudentsAsync()
        {
            var universityId = _userService.GetUserId();
            var students = await (from user in _userManager.Users
                                  join student in _context.Students
                                  on user.Id equals student.UserId
                                  where user.UniversityId == universityId && user.Role == "student"
                                  select new StudentResponse
                                  {
                                      UserId = user.Id,
                                      UniversityId = user.UniversityId,
                                      FirstName = user.FirstName,
                                      LastName = user.LastName,
                                      IsApproved = user.IsApproved,
                                      Address = user.Address,
                                      Email = user.Email,
                                      CreateOn = user.CreateOn,
                                      Role = user.Role,
                                      StudentId = student.StudentId,
                                      RollNo = student.RollNo,
                                      Dob = student.Dob,
                                      Gender = student.Gender,
                                      MaritalStatus = student.MaritalStatus,
                                      JoiningDate = student.JoiningDate,
                                      GraduationDate = student.GraduationDate
                                  }).ToListAsync();
            return students;
        }
        public async Task<StudentResponse> GetStudentByIdAsync(Guid userId)
        {
            var universityId = _userService.GetUserId();
            var students = await (from user in _userManager.Users
                         join student in _context.Students
                         on user.Id equals student.UserId
                         where user.UniversityId == universityId && user.Role == "student" && user.Id == userId
                         select new StudentResponse
                         {
                             UserId = user.Id,
                             UniversityId = user.UniversityId,
                             FirstName = user.FirstName,
                             LastName = user.LastName,
                             IsApproved = user.IsApproved,
                             Address = user.Address,
                             Email = user.Email ?? "",
                             CreateOn = user.CreateOn,
                             Role = user.Role,
                             StudentId = student.StudentId,
                             RollNo = student.RollNo,
                             Dob = student.Dob,
                             Gender = student.Gender,
                             MaritalStatus = student.MaritalStatus,
                             JoiningDate = student.JoiningDate,
                             GraduationDate = student.GraduationDate
                         }).FirstOrDefaultAsync();
            return students;
        }
        public async Task<IdentityResult> AddStudent(StudentRequest studentRequest)
        {
            ApplicationUser newUser = new ApplicationUser
            {
                //Id = Guid.NewGuid(),
                UniversityId = _userService.GetUserId(),
                FirstName = studentRequest.FirstName,
                LastName = studentRequest.LastName,
                IsApproved = true,
                Address = studentRequest.Address,
                Email = studentRequest.Email,
                UserName = studentRequest.Email,
                CreateOn = DateTime.Now,
                Role = "student",
            };
            IdentityResult result = await _userManager.CreateAsync(newUser, studentRequest.Password);
            if (!result.Succeeded)
            {
                return result;
            }
            await _userManager.AddToRoleAsync(newUser, UserRoles.Student);
            await _emailService.SendEmailConfirmationAsync(newUser);

            var student = new Student
            {
                StudentId = Guid.NewGuid(),
                UserId = newUser.Id,
                RollNo = studentRequest.RollNo,
                Dob = studentRequest.Dob,
                Gender = studentRequest.Gender,
                MaritalStatus = studentRequest.MaritalStatus,
                JoiningDate = studentRequest.JoiningDate,
                GraduationDate = studentRequest.GraduationDate,
            };
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return result;
        }
        public async Task<IdentityResult> UpdateStudentAsync(Guid studentId, StudentRequest studentRequest)
        {
            var student = await _userManager.FindByIdAsync(studentId.ToString());
            if (student == null) { return IdentityResult.Failed(new IdentityError { Description = $"Student with ID '{studentId}' not found." }); }
            student.FirstName = studentRequest.FirstName;
            student.LastName = studentRequest.LastName;
            student.IsApproved = true;
            student.Address = studentRequest.Address;
            student.Email = studentRequest.Email;
            student.UserName = studentRequest.Email;
            student.UpdateOn = DateTime.Now;

            var updateResult = await _userManager.UpdateAsync(student);
            if (!updateResult.Succeeded) { return IdentityResult.Failed(new IdentityError { Description = $"Failed to update student with Name '{student.FirstName}'." }); }
            var stdDetails = await _context.Students.Where(s => s.UserId == student.Id).FirstOrDefaultAsync();
            if (stdDetails == null) { return IdentityResult.Failed(new IdentityError { Description = $"Student details not found for user ID '{student.Id}'." }); }
            stdDetails.RollNo = studentRequest.RollNo;
            stdDetails.Dob = studentRequest.Dob;
            stdDetails.Gender = studentRequest.Gender;
            stdDetails.MaritalStatus = studentRequest.MaritalStatus;
            stdDetails.JoiningDate = studentRequest.JoiningDate;
            stdDetails.GraduationDate = studentRequest.GraduationDate;

            var status = await _context.SaveChangesAsync();
            if (status == 0)
                return IdentityResult.Failed(new IdentityError { Description = $"Student with ID '{student.Id}' not found." });
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteStudent(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null && user.Role == UserRoles.Student.ToLower())
            {
                var result = await _userManager.DeleteAsync(user);
                await _context.SaveChangesAsync();
                return result;
            }
            return IdentityResult.Failed(new IdentityError { Description = $"Student with ID '{id}' not found." });
        }
    }
}
