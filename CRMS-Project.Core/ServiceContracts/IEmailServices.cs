using CRMS_Project.Core.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.ServiceContracts
{
    public interface IEmailServices
    {
        Task SendEmailConfirmationAsync(ApplicationUser user);
    }
}
