using CRMS_Project.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace CRMS_Project.Core.DTO.Response
{
    public class JobPostingResponse:AuthenticationResponse
    {
        public Guid JobId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UniversityId { get; set; }
        public List<StudentCourse> Courses { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? RejectedDate { get; set; }
        public DateTime Deadline { get; set; }
        public string Document { get; set; }
        public ApplicationStatus Status { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreateOn { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? UpdateOn { get; set; }
    }
}
