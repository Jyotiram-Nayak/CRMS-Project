using CRMS_Project.Core.Domain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using CRMS_Project.Core.Enums;

namespace CRMS_Project.Core.Domain.Entities
{
    public class JobApplication
    {
        [Key]
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
        public string Resume { get; set; }
        public string? AssessmentLink { get; set; } 
        public bool? AssessmentCompleted { get; set; } 
        public DateTime? AssessmentCompletionDate { get; set; } 
        public string? AssessmentScore { get; set; } 
        public string? AssessmentFeedback { get; set; } 
        [Required]
        public DateTime CreateOn { get; set; }
        public DateTime? UpdateOn { get; set; }
        // Navigation properties
        [ForeignKey("JobId")]
        public virtual JobPosting JobPosting { get; set; }
        [ForeignKey("StudentId")]
        public virtual ApplicationUser Student { get; set; }
        [ForeignKey("CompanyId")]
        public virtual ApplicationUser Company { get; set; }
        [ForeignKey("UniversityId")]
        public virtual ApplicationUser University { get; set; }
    }
}
