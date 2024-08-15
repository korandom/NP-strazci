using App.Server.Services.Authorization;
using App.Server.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using App.Server.DTOs;
using App.Server.Models.Identity;

namespace App.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IAppAuthenticationService authenticationService, IAppAuthorizationService authorizationService) : ControllerBase
    {
        private readonly IAppAuthenticationService _authenticationService = authenticationService;
        private readonly IAppAuthorizationService _authorizationService = authorizationService;

        // Registering a new user connected to ranger
        [Authorize(Policy = "RequireHeadOfRangerRole,RequireAdminRole")]
        [HttpPut("register-user")]
        public async Task<IActionResult> RegisterRangerUser(RangerDto rangerDto)
        {
            var user = new ApplicationUser
            {
                UserName = rangerDto.Email,
                Email = rangerDto.Email,
                EmailConfirmed = true, // TODO: implement email confiramtions?
                RangerId = rangerDto.Id
            };
            var result = await _authenticationService.RegisterUserAsync(user);

            if (result.Succeeded) {
                await _authorizationService.AssignRoleAsync(user, "Ranger");
            }

            return Ok(result);
        }

        // Admin can assign roles to users
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("assign-role/{userEmail}")]
        public async Task<IActionResult> AssignRole(string userEmail, string role)
        {
            var user = await _authenticationService.GetUserAsync(userEmail);
            if (user == null)
            {
                return NotFound($"User with email {userEmail} was not found.");
            }
            var result = await _authorizationService.AssignRoleAsync(user, role);


            if(result == null)
            {
                return BadRequest("Assigning role was not succesfull");
            }

            if(result.Succeeded) 
            {
                return Ok($"Successfully assigned role{role} to {userEmail}");
            }

            return BadRequest(result.ToString());
        }

        // signin
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(string email, string password)
        {
            var result = await _authenticationService.SignInAsync(email, password);

            if (result.Succeeded)
            {
                return Ok("Sign-in successful");
            }

            return Unauthorized("Invalid email or password.");
        }
        // signout
        [Authorize]
        [HttpPost("signout")]
        public async Task<IActionResult> SignOutt()
        {
            await _authenticationService.SignOutAsync();
            return Ok("Sign-out successful");
        }
        // getuser
        // set password -- user created with admin and is setting up first passwrod via link with token and email ? 
        // change password

    }
}
