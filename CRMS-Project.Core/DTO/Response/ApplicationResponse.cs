using CRMS_Project.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.DTO.Response
{
    public class ApplicationResponse:AuthenticationResponse
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UniversityId { get; set; }
        public ApplicationStatus Status { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateSubmitted { get; set; }
    }
}
