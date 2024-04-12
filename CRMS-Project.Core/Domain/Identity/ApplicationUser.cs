using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CRMS_Project.Core.Domain.Identity
{
    public class ApplicationUser : IdentityUser  
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string? UniversityId { get; set; }
        [Required]
        public bool IsApproved { get; set; }
        public string? Image { get; set; }
        public string Address { get; set; }
        public string? Website { get; set; }
        public string Role { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreateOn { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? UpdateOn { get; set; }
    }
}
