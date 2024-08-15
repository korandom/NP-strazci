using App.Server.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace App.Server.Util
{
    public static class DataSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "HeadOfDistrict", "Ranger" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        // Called after SeedRolesAsync
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string adminEmail = "admin@admin.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            
            // Admin is already seeded in system
            if (adminUser != null)
            {
                return;
            }
            
            adminUser = new ApplicationUser {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            

            var result = await userManager.CreateAsync(adminUser, "DebugAdminPass123.");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
