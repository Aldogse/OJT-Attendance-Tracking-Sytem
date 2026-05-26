using Attendace_Tracking_Sytem.Enums;
using Microsoft.AspNetCore.Identity;

namespace Attendace_Tracking_Sytem.Seeders
{
    public class RoleSeeder
    {
        public static async Task seedRolesAsync(RoleManager<IdentityRole>roles)
        {
            foreach (var role in Enum.GetValues<Roles>())
            {
                if(!await roles.RoleExistsAsync(role.ToString()))
                {
                    await roles.CreateAsync(new IdentityRole(role.ToString()));
                }
            }
        }
    }
}
