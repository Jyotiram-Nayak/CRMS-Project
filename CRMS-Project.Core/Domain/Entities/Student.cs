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
        public string StudentId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string RollNo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Dob { get; set; }
        public GenderOptions? Gender { get; set; }
        public MaritalOptions? MaritalStatus { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime JoiningDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? GraduationDate { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
    
}
