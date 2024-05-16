using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.DTO.Response
{
    public class UniversityDashboardResponse
    {
        public int SelectedStudents { get; set; }
        public int PendingStudents { get; set; }
        public int AllStudents { get; set; }
        public int ApplyedCompanys { get; set; }
    }
}
