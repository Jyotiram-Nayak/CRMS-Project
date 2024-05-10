using CRMS_Project.Core.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.DTO.Request
{
    public class JobPostingRequest
    {
        public Guid UniversityId { get; set; }
        [Required(ErrorMessage = "Courses is required.")]
        public StudentCourse[] Courses { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Deadline is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Deadline { get; set; }
        [Required(ErrorMessage = "Document is required.")]
        public string Document { get; set; }
    }
}
