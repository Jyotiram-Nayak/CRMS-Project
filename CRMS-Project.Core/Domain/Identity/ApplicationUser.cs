using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CRMS_Project.Core.Domain.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public Guid? UniversityId { get; set; }
        [Required]
        public bool IsApproved { get; set; }
        public string? Image { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        public string? Website { get; set; }
        public string? Bio { get; set; } 
        [Required]
        public string Role { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreateOn { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? UpdateOn { get; set; }
    }
}
