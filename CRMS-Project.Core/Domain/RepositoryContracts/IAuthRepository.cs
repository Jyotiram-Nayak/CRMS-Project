using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.DTO.Request;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.Domain.RepositoryContracts
{
    public interface IAuthRepository
    {
        Task<IdentityResult> RegisterUserAsync(RegisterRequest registerRequest);
    }
}
