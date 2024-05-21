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
    public class JobApplicationResponse
    {
        public Guid ApplicationId { get; set; }
        [Required]
        public Guid JobId { get; set; }
        [Required]
        public Guid StudentId { get; set; }
        [Required]
        public Guid CompanyId { get; set; }
        [Required]
        public Guid UniversityId { get; set; }
        [Required]
        public DateTime AppliedDate { get; set; }
        public DateTime? InterviewDate { get; set; }
        [Required]
        public SelectionStatus isSelected { get; set; }
        public StudentCourse? Course { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Resume { get; set; }
        public string? AssessmentLink { get; set; }
        public bool? AssessmentCompleted { get; set; }
        public DateTime? AssessmentCompletionDate { get; set; }
        public string? AssessmentScore { get; set; }
        public string? AssessmentFeedback { get; set; }
        public string? JobTitle { get; set; }
        public string? StudentName { get; set; }
        public string? StudentEmail { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyEmail { get; set; }
        public string? UniversityName { get; set; }

        [Required]
        public DateTime CreateOn { get; set; }
        public DateTime? UpdateOn { get; set; }
    }
}
