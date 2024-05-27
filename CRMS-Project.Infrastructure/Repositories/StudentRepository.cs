using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.Domain.RepositoryContracts;
using CRMS_Project.Core.DTO;
using CRMS_Project.Core.DTO.Request;
using CRMS_Project.Core.DTO.Response;
using CRMS_Project.Core.Enums;
using CRMS_Project.Core.ServiceContracts;
using CRMS_Project.Infrastructure.DbContext;
using ExcelDataReader;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
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
        public async Task<List<StudentResponse>> GetAllStudentsAsync(PaginationParameters parameters)
        {
            try
            {
                var universityId = _userService.GetUserId();
                var query = from user in _userManager.Users
                            join student in _context.Students
                            on user.Id equals student.UserId
                            where user.UniversityId == universityId && user.Role == "student"
                            orderby user.CreateOn descending
                            select new StudentResponse
                            {
                                UserId = user.Id,
                                UniversityId = user.UniversityId,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                IsApproved = user.IsApproved,
                                Address = user.Address,
                                City = user.City,
                                State = user.State,
                                Email = user.Email ?? "",
                                PhoneNumber = user.PhoneNumber ?? "",
                                CreateOn = user.CreateOn,
                                Role = user.Role,
                                Course = user.Course,
                                StudentId = student.StudentId,
                                RollNo = student.RollNo,
                                Dob = student.Dob,
                                Gender = student.Gender,
                                MaritalStatus = student.MaritalStatus,
                                JoiningDate = student.JoiningDate,
                                GraduationDate = student.GraduationDate,
                                IsSelected = student.IsSelected,
                                Image = user.Image
                            };
                if (!string.IsNullOrEmpty(parameters.FilterOn) && !string.IsNullOrEmpty(parameters.FilterQuery))
                {
                    var filterOn = parameters.FilterOn.Trim().ToLowerInvariant();
                    var filterQuery = parameters.FilterQuery.Trim();
                    switch (filterOn)
                    {
                        case "firstname":
                            query = query.Where(student => student.FirstName.Contains(filterQuery));
                            break;
                        case "rollno":
                            query = query.Where(student => student.RollNo.Contains(filterQuery));
                            break;
                        case "email":
                            query = query.Where(student => student.Email.Contains(filterQuery));
                            break;
                        case "city":
                            query = query.Where(student => student.City.Contains(filterQuery));
                            break;
                        case "state":
                            query = query.Where(student => student.State.Contains(filterQuery));
                            break;
                        case "course":
                            if (Enum.TryParse(filterQuery, out StudentCourse course))
                            {
                                query = query.Where(student => student.Course == course);
                            }
                            break;
                        case "isselected":
                            var selected = filterQuery == "true" ? true : false;
                            query = query.Where(student => student.IsSelected == selected);
                            break;
                        default:
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(parameters.SortBy))
                {
                    switch (parameters.SortBy.ToLower())
                    {
                        case "firstname":
                            query = parameters.IsAscending ? query.OrderBy(student => student.FirstName) : query.OrderByDescending(student => student.FirstName);
                            break;
                        case "createon":
                            query = parameters.IsAscending ? query.OrderBy(student => student.CreateOn) : query.OrderByDescending(student => student.CreateOn);
                            break;
                        case "city":
                            query = parameters.IsAscending ? query.OrderBy(student => student.City) : query.OrderByDescending(student => student.City);
                            break;
                        case "state":
                            query = parameters.IsAscending ? query.OrderBy(student => student.State) : query.OrderByDescending(student => student.State);
                            break;
                        default:
                            query = query.OrderByDescending(student => student.CreateOn);
                            break;
                    }
                }
                else
                {
                    query = query.OrderByDescending(student => student.CreateOn);
                }
                var paginatedQuery = query.Skip((parameters.Page - 1) * parameters.PageSize)
                                                 .Take(parameters.PageSize);

                var students = await paginatedQuery.ToListAsync();
                return students;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<StudentResponse> GetStudentByIdAsync(Guid userId)
        {
            try
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
                                          City = user.City,
                                          State = user.State,
                                          Email = user.Email ?? "",
                                          PhoneNumber = user.PhoneNumber ?? "",
                                          CreateOn = user.CreateOn,
                                          Role = user.Role,
                                          Course = user.Course,
                                          StudentId = student.StudentId,
                                          RollNo = student.RollNo,
                                          Dob = student.Dob,
                                          Gender = student.Gender,
                                          MaritalStatus = student.MaritalStatus,
                                          JoiningDate = student.JoiningDate,
                                          GraduationDate = student.GraduationDate,
                                          IsSelected = student.IsSelected
                                      }).FirstOrDefaultAsync();
                return students;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<IdentityResult> AddStudent(StudentRequest studentRequest)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(studentRequest.Email);
                if (user != null) return IdentityResult.Failed(new IdentityError { Description = "User already exist" });
                ApplicationUser newUser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UniversityId = _userService.GetUserId(),
                    FirstName = studentRequest.FirstName,
                    LastName = studentRequest.LastName,
                    IsApproved = true,
                    Address = studentRequest.Address,
                    PhoneNumber = studentRequest.PhoneNumber,
                    City = studentRequest.City,
                    State = studentRequest.State,
                    Email = studentRequest.Email,
                    UserName = studentRequest.Email,
                    CreateOn = DateTime.Now,
                    Role = "student",
                };
                if (Enum.TryParse<StudentCourse>(studentRequest.Course?.ToString(), out var course))
                {
                    newUser.Course = course;
                }
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
                    JoiningDate = studentRequest.JoiningDate,
                    GraduationDate = studentRequest.GraduationDate,
                    IsSelected = false,
                };
                if (Enum.TryParse<GenderOptions>(studentRequest.Gender?.ToString(), out var gender))
                {
                    student.Gender = gender;
                }
                if (Enum.TryParse<MaritalOptions>(studentRequest.MaritalStatus?.ToString(), out var IsMarried))
                {
                    student.MaritalStatus = IsMarried;
                }
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }
        public async Task<IdentityResult> UpdateStudentAsync(Guid studentId, UpdateStudentRequest studentRequest)
        {
            try
            {
                var student = await _userManager.FindByIdAsync(studentId.ToString());
                if (student == null) { return IdentityResult.Failed(new IdentityError { Description = $"Student with ID '{studentId}' not found." }); }
                student.FirstName = studentRequest.FirstName;
                student.LastName = studentRequest.LastName;
                student.IsApproved = true;
                if (!string.IsNullOrEmpty(studentRequest.Password))
                {
                    var newPasswordHash = _userManager.PasswordHasher.HashPassword(student, studentRequest.Password);
                    student.PasswordHash = newPasswordHash;
                }
                student.Address = studentRequest.Address;
                student.City = studentRequest.City;
                student.State = studentRequest.State;
                student.Email = studentRequest.Email;
                student.PhoneNumber = studentRequest.PhoneNumber;
                student.UserName = studentRequest.Email;
                student.UpdateOn = DateTime.Now;

                if (Enum.TryParse<StudentCourse>(studentRequest.Course?.ToString(), out var course))
                {
                    student.Course = course;
                }
                var updateResult = await _userManager.UpdateAsync(student);
                await _context.SaveChangesAsync();

                if (!updateResult.Succeeded) { return IdentityResult.Failed(new IdentityError { Description = $"Failed to update student with Name '{student.FirstName}'." }); }
                var stdDetails = await _context.Students.Where(s => s.UserId == student.Id).FirstOrDefaultAsync();
                if (stdDetails == null) { return IdentityResult.Failed(new IdentityError { Description = $"Student details not found for user ID '{student.Id}'." }); }

                _context.Entry(stdDetails).State = EntityState.Modified;
                stdDetails.RollNo = studentRequest.RollNo;
                stdDetails.Dob = studentRequest.Dob;
                stdDetails.JoiningDate = studentRequest.JoiningDate;
                stdDetails.GraduationDate = studentRequest.GraduationDate;
                if (Enum.TryParse<GenderOptions>(studentRequest.Gender?.ToString(), out var gender))
                {
                    stdDetails.Gender = gender;
                }
                if (Enum.TryParse<MaritalOptions>(studentRequest.MaritalStatus?.ToString(), out var IsMarried))
                {
                    stdDetails.MaritalStatus = IsMarried;
                }
                var status = await _context.SaveChangesAsync();
                if (status == 0)
                    return IdentityResult.Failed(new IdentityError { Description = $"Student with ID '{student.Id}' not found." });
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }

        public async Task<IdentityResult> DeleteStudent(Guid id)
        {
            try
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
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }
        public async Task<IdentityResult> ImportExcelFile(string fileUrl)
        {
            IdentityResult result = new IdentityResult();
            int totalRecords = 0;
            int errorCount = 0;
            try
            {
                string fileName = "temp.xlsx"; // Temporary file name
                string rootPath = Directory.GetCurrentDirectory(); // Get the root directory path
                string filePath = Path.Combine(rootPath, "wwwroot", fileName);
                using (var client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(new Uri(fileUrl), filePath);
                }
                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                //using (var stream = new StreamReader(tempFilePath, Encoding.UTF8))
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            reader.Read();
                            while (reader.Read())
                            {
                                totalRecords++;
                                try
                                {
                                    var email = reader.GetValue(3)?.ToString();
                                    if (string.IsNullOrEmpty(email) || await _userManager.FindByEmailAsync(email) != null)
                                    {
                                        continue; // Skip this record if email is null or already exists
                                    }
                                    ApplicationUser newUser = new ApplicationUser
                                    {
                                        Id = Guid.NewGuid(),
                                        UniversityId = _userService.GetUserId(),
                                        FirstName = reader.GetValue(1)?.ToString() ?? "",
                                        LastName = reader.GetValue(2)?.ToString() ?? "",
                                        Email = reader.GetValue(3)?.ToString() ?? "",
                                        PhoneNumber = reader.GetValue(4)?.ToString() ?? "",
                                        IsApproved = true,
                                        Address = reader.GetValue(12)?.ToString() ?? "",
                                        City = reader.GetValue(13)?.ToString() ?? "",
                                        State = reader.GetValue(14)?.ToString() ?? "",
                                        UserName = reader.GetValue(3)?.ToString() ?? "",
                                        CreateOn = DateTime.Now,
                                        Role = "student",
                                    };
                                    // parse course
                                    if (Enum.TryParse<StudentCourse>(reader.GetValue(11)?.ToString(), out var course))
                                    {
                                        newUser.Course = course;
                                    }
                                    var password = reader.GetValue(5).ToString() ?? "";
                                    result = await _userManager.CreateAsync(newUser, password);
                                    if (result.Succeeded)
                                    {
                                        await _userManager.AddToRoleAsync(newUser, UserRoles.Student);
                                        await _emailService.SendEmailConfirmationAsync(newUser);

                                        var student = new Student
                                        {
                                            StudentId = Guid.NewGuid(),
                                            UserId = newUser.Id,
                                            RollNo = reader.GetValue(7)?.ToString() ?? "",
                                            IsSelected = false,
                                        };
                                        if (Enum.TryParse<GenderOptions>(reader.GetValue(9)?.ToString(), out var gender))
                                        {
                                            student.Gender = gender;
                                        }
                                        if (Enum.TryParse<MaritalOptions>(reader.GetValue(10)?.ToString(), out var maritalStatus))
                                        {
                                            student.MaritalStatus = maritalStatus;
                                        }
                                        if (DateTime.TryParse(reader.GetValue(8)?.ToString(), out var dob))
                                        {
                                            student.Dob = dob;
                                        }
                                        if (DateTime.TryParse(reader.GetValue(15)?.ToString(), out var joiningDate))
                                        {
                                            student.JoiningDate = joiningDate;
                                        }
                                        if (DateTime.TryParse(reader.GetValue(16)?.ToString(), out var graduationDate))
                                        {
                                            student.GraduationDate = graduationDate;
                                        }
                                        _context.Students.Add(student);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                                catch (Exception)
                                {
                                    errorCount++;
                                    continue;
                                }
                            }
                        } while (reader.NextResult());
                    }
                }
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                result = IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
            return result;
        }
    }
}
