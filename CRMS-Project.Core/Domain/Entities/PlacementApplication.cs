using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.Domain.Entities
{
    public class PlacementApplication
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string CompanyId { get; set; }
        [Required]
        public string UniversityId { get; set; }
        [Required]
        public ApplicationStatus Status { get; set; } 
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateSubmitted { get; set; }
    }
}
