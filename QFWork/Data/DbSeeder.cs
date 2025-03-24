using Microsoft.AspNetCore.Identity;
using QFWork.Constants;

namespace QFWork.Data
{
    public class DbSeeder
    {
        internal static async Task SeedRoles(IServiceProvider service)
        {
            var roleManager = service.GetService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole(Roles.Student.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Teacher.ToString()));
        }
    }
}
