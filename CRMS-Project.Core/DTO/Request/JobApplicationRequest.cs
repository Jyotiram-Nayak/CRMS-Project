using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.DTO.Request
{
    public class JobApplicationRequest
    {
        [Required]
        public Guid JobId { get; set; }
        [Required]
        public string Resume { get; set; }
    }
}
