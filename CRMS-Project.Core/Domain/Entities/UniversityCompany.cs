using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.Domain.Identity
{
    public class UniversityCompany
    {
        [Key]
        public Guid UniversityCompanyID { get; set; }

        public Guid UniversityID { get; set; }
        [ForeignKey("UniversityID")]
        public University University { get; set; }

        public Guid CompanyID { get; set; }
        [ForeignKey("CompanyID")]
        public Company Company { get; set; }
    }
}
