using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.DTO.Response
{
    public class AdminDashboardResponse
    {
        public int TotalUniversities { get; set; }
        public int TotalCompanies { get; set; }
        public int TotalStudents { get; set; }
    }
}
