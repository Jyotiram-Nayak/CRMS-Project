using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.ServiceContracts
{
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(ApplicationUser user);
        Task SendForgotEmailAsync(ApplicationUser user);
        Task SendContactusEmailAsync(ApplicationUser user, ContactUsRequest contact);
    }
}
