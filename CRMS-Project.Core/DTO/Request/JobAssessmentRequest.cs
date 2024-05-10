using CRMS_Project.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.DTO.Request
{
    public class JobAssessmentRequest
    {
        public DateTime? InterviewDate { get; set; }
        public SelectionStatus? isSelected { get; set; }
        public string? AssessmentLink { get; set; }
        public bool? AssessmentCompleted { get; set; }
        public string? AssessmentScore { get; set; }
        public string? AssessmentFeedback { get; set; }
    }
}
