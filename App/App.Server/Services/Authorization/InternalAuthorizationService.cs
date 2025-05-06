using App.Server.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace App.Server.Services.Authorization
{
    /// <summary>
    /// Provides role-based authorization functionality for application users.
    /// </summary>
    public class InternalAuthorizationService(UserManager<ApplicationUser> userManager) : IAppAuthorizationService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        /// <summary>
        /// Assigns a specified role to a given user.
        /// </summary>
        /// <param name="user">The user to whom the role will be assigned.</param>
        /// <param name="role">The name of the role to assign.</param>
        /// <returns>An IdentityResult indicating the result of the operation.</returns>
        public async Task<IdentityResult> AssignRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        /// <summary>
        /// Retrieves the role of the user.
        /// Needs updating when adding roles.
        /// </summary>
        /// <param name="user">The user whose role is to be determined.</param>
        /// <returns>The name of the user's role if found else empty.</returns>
        public async Task<string> GetRoleAsync(ApplicationUser user)
        {
            string[] roles = { "Admin", "HeadOfDistrict", "Ranger" };
            foreach (string role in roles)
            {
                if (await _userManager.IsInRoleAsync(user, role))
                {
                    return role;
                }
            }
            return "";
        }

        /// <summary>
        /// Checks whether a user belongs to a specific role.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <param name="role">The role name to verify.</param>
        /// <returns>True if the user is in the specified role else false.</returns>
        public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        /// <summary>
        /// Checks whether the given user has the rangerID.
        /// </summary>
        public bool IsUserOwner(ApplicationUser user, int rangerId)
        {
            return user != null && user.RangerId == rangerId;
        }

    }
}
