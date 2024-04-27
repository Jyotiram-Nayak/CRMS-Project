using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.DTO.Response
{
    public class AuthenticationResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string? Bio { get; set; }
        public string? Image { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime? UpdateOn { get; set; }
    }
}
