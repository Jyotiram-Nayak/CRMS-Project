using CRMS_Project.Core.Domain.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.Domain.Entities
{
    public class JobPosting
    {
        [Key]
        public Guid JobId { get; set; }
        [Required]
        public Guid CompanyId { get; set; }
        [Required]
        public Guid UniversityId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime PostedDate { get; set; }
        [Required]
        public DateTime Deadline { get; set; }
        [Required]
        public string Document { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreateOn { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? UpdateOn { get; set; }

        [ForeignKey("CompanyId")]
        public virtual ApplicationUser Company { get; set; }

        [ForeignKey("UniversityId")]
        public virtual ApplicationUser University { get; set; }
    }
}
