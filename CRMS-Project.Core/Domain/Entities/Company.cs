using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.Domain.Entities
{
    internal class Company
    {
        [Key]
        public Guid CompanyId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Location is required")]
        [StringLength(100)]
        public string Location { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits")]
        public string PhoneNo { get; set; } // Changed data type to string
        [Required(ErrorMessage = "Website is required")]
        [Url(ErrorMessage = "Invalid URL")]
        public string Website { get; set; }
        [Required(ErrorMessage = "CreatedOn date is required")]
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public virtual ICollection<UniversityCompany> UniversityCompanies { get; set; }
    }
}
