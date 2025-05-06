using App.Server.Models.AppData;
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
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            string? adminEmail = configuration.GetSection("SeedAdminUser")["Email"];
            string? adminPass = configuration.GetSection("SeedAdminUser")["Password"];

            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPass))
            {
                throw new Exception("Admin email or password not configured in appsettings.json.");
            }
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            // Admin is already seeded in system
            if (adminUser != null)
            {
                return;
            }

            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPass);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        public static async Task SeedDistrictsAsync(IServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetRequiredService<PlannerNPContext>();

            District[] districts = [
                new District { Id = 1, Name = "Stožec" },
                new District { Id = 2, Name = "Modrava" }
            ];
            foreach (var district in districts)
            {
                var d = db.Districts.Find(district.Id);
                if(d == null)
                {
                    db.Add(district);
                }
            }
            await db.SaveChangesAsync();
        }

    }
}
