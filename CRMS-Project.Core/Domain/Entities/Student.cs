using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CRMS_Project.Core.Enums;

namespace CRMS_Project.Core.Domain.Identity
{
    public class Student
    {
        [Key]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required")]
        public DateTime Dob { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public GenderOptions Gender { get; set; }

        [Required(ErrorMessage = "Marital status is required")]
        public MaritalOptions MaritalStatus { get; set; }
        [Required]
        public DateTime JoiningDate { get; set; }
        [Required]
        public DateTime GraduationDate { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
    
}
