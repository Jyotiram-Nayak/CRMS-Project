using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.DTO.Response
{
    public class ApprovedUniversityResponse
    {
        public Guid UniversityId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}
