using App.Server.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace App.Server.Services.Authentication
{
    public interface IAppAuthenticationService
    {
        Task<SignInResult> SignInAsync(string username, string password);
        Task SignOutAsync();
        Task<IdentityResult> RegisterUserAsync(ApplicationUser user);
        Task<ApplicationUser?> GetUserAsync(string email);
        Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal);
    }
}
