using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CRMS_Project.Infrastructure.DbContext
{
    public class AppDbInitializer
    {
        public static async Task InitializerAsync(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = { UserRoles.Admin, UserRoles.University, UserRoles.Company, UserRoles.Student };
            IdentityResult roleResult;
            foreach (var role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            var email = "admin@gmail.com";
            var password = "Admin@123";
            if (userManager.FindByEmailAsync(email).Result == null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    IsApproved=true,
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true
                };
                IdentityResult result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
