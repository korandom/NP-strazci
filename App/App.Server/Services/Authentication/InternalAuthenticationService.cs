using App.Server.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace App.Server.Services.Authentication
{
    /// <summary>
    /// Provides internal authentication logic for signing in, signing out,
    /// registering users, and retrieving user information.
    /// </summary>
    public class InternalAuthenticationService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) : IAppAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        /// <summary>
        /// Attempts to sign in a user with the provided username and password.
        /// </summary>
        /// <param name="username">The username of the user attempting to sign in.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>A <see cref="SignInResult"/> indicating the result of the sign-in attempt.</returns>

        public async Task<SignInResult> SignInAsync(string username, string password)
        {
            return await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);
        }


        /// <summary>
        /// Signs out the signed-in user.
        /// </summary>
        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        /// <summary>
        /// Registers a new user in the system with a temporary password.
        /// </summary>
        /// <param name="user">The user to register.</param>
        /// <returns>An <see cref="IdentityResult"/> indicating the success or failure of the registration.</returns>
        /// <remarks>
        /// This implementation assigns a temporary password. This needs to be adressed when switching to lapd.
        /// </remarks>
        public async Task<IdentityResult> RegisterUserAsync(ApplicationUser user)
        {
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                await _userManager.AddPasswordAsync(user, "tempPass123.");
            }
            return result;
        }
        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>The user if found else null.</returns>
        public async Task<ApplicationUser?> GetUserAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// Retrieves a user based on ClaimsPrincipal./>.
        /// </summary>
        /// <param name="principal">The claims principal representing the current user.</param>
        /// <returns>The user if found else null.</returns>
        public async Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal)
        {
            return await _userManager.GetUserAsync(principal);
        }
    }
}
