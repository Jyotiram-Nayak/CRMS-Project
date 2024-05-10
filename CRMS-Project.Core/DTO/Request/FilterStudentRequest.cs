using CRMS_Project.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.DTO.Request
{
    public class FilterStudentRequest
    {
        public string? Course { get; set; }
        public string? IsSelected { get; set; } = null;

    }
}
