using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.DTO.Response
{
    public class CompanyDashboardResponse
    {
        public int TotalJobs { get; set; }
        public int TotalApplication { get; set; }
        public int PendingStudents { get; set; }
        public int SelectedStudents { get; set; }
        public int RejectedStudents { get; set; }
    }
}
