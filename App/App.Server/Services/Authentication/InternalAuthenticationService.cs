using App.Server.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace App.Server.Services.Authentication
{
    public class InternalAuthenticationService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) : IAppAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<SignInResult> SignInAsync(string username, string password)
        {
            return await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> RegisterUserAsync(ApplicationUser user)
        {
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                await _userManager.AddPasswordAsync(user, "tempPass123.");
                // TODO: instead send an email to the user to set their password?
            }
            return result;
        }

        public async Task<ApplicationUser?> GetUserAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal)
        {
            return await _userManager.GetUserAsync(principal);
        }
    }
}
