using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.Domain.Entities
{
    internal class ApplicationUser : IdentityUser
    {
        public string? Image { get; set; }
        [Required]
        public bool IsApproved { get; set; }
    }
}
