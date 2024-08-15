using App.Server.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace App.Server.Services.Authorization
{
    public interface IAppAuthorizationService
    {
        Task<IdentityResult> AssignRoleAsync(ApplicationUser user, string role);
        Task<bool> IsInRoleAsync(ApplicationUser user, string role);
        bool IsUserOwner(ApplicationUser user, int rangerId);
    }
}
