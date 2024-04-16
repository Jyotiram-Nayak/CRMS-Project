using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.Domain.Entities
{
    public class PlacementApplication
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid CompanyId { get; set; }
        [Required]
        public Guid UniversityId { get; set; }
        [Required]
        public ApplicationStatus Status { get; set; } 
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateSubmitted { get; set; }

        [ForeignKey("CompanyId")]
        public virtual ApplicationUser Company { get; set; }

        [ForeignKey("UniversityId")]
        public virtual ApplicationUser University { get; set; }
    }
}
