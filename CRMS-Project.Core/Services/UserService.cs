using CRMS_Project.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CRMS_Project.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public Guid GetUserId()
        {
            var userId = new Guid(_contextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier));
            return userId;
        }
        public string GetUserRole()
        {
            return _contextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.Role);
        }
    }
}
