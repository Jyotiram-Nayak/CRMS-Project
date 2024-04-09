using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CRMS_Project.Core.Domain.Identity
{
    public class ApplicationUser : IdentityUser   
    {

        public string? Image { get; set; }
        [Required]
        public bool IsApproved { get; set; }
    }
}
