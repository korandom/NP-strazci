using App.Server.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace App.Server.Services.Authorization
{
    public class InternalAuthorizationService(UserManager<ApplicationUser> userManager) : IAppAuthorizationService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<IdentityResult> AssignRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<string> GetRoleAsync(ApplicationUser user)
        {
            string[] roles = { "Admin", "HeadOfDistrict", "Ranger" };
            foreach (string role in roles)
            {
                if(await _userManager.IsInRoleAsync(user, role))
                {
                    return role;
                }
            }
            return "";
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public bool IsUserOwner(ApplicationUser user, int rangerId)
        {
            return user!= null && user.RangerId == rangerId;
        }

    }
}
