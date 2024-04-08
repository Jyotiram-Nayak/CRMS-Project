using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.Domain.Entities
{
    internal class Student
    {
        [Key]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        public DateTime Dob { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Marital status is required")]
        public MaritalStatus MaritalStatus { get; set; }

        [Required(ErrorMessage = "Joining date is required")]
        public DateTime JoiningDate { get; set; }

        [Required(ErrorMessage = "Graduation date is required")]
        public DateTime GraduationDate { get; set; }

        [Required(ErrorMessage = "CreatedOn date is required")]
        public DateTime CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
    public enum Gender
    {
        Male, Female, Other
    }
    public enum MaritalStatus
    {
        Married, Unmarried
    }
}
