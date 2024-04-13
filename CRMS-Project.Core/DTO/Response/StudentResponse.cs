using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.DTO.Response
{
    public class StudentResponse : AuthenticationResponse
    {
        public Guid StudentId { get; set; }
        public Guid UserId { get; set; }
        public Guid? UniversityId { get; set; }
        public string RollNo { get; set; }
        public DateTime? Dob { get; set; }
        public GenderOptions? Gender { get; set; }
        public MaritalOptions? MaritalStatus { get; set; }
        public DateTime JoiningDate { get; set; }
        public DateTime? GraduationDate { get; set; }
    }
}
