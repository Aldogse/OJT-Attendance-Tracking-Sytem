using Attendace_Tracking_Sytem.Enums;
using Microsoft.AspNetCore.Identity;

namespace Attendace_Tracking_Sytem.Seeders
{
    public class RoleSeeder
    {
        public static async Task SeedRoleAsync(RoleManager<IdentityRole>roleManager)
        {
            foreach (var role in Enum.GetNames(typeof(Roles)))
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
