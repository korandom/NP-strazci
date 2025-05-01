using App.Server.DTOs;
using App.Server.Models.Identity;
using App.Server.Services.Authentication;
using App.Server.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Server.Controllers
{

    /// <summary>
    /// Api controller for managing users, their roles and signing in/out.
    /// </summary>
    /// <param name="authenticationService">Injecting authentication service.</param>
    /// <param name="authorizationService">Injecting authorization service.</param>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IAppAuthenticationService authenticationService, IAppAuthorizationService authorizationService) : ControllerBase
    {
        private readonly IAppAuthenticationService _authenticationService = authenticationService;
        private readonly IAppAuthorizationService _authorizationService = authorizationService;

        /// <summary>
        /// Registering a new user connected to ranger.
        /// </summary>
        /// <param name="rangerDto">Ranger that represents the user.</param>
        /// <returns>Status code 200 when succesfful.</returns>
        [Authorize(Roles = "Admin,HeadOfDistrict")]
        [HttpPut("register-user")]
        public async Task<ActionResult> RegisterRangerUser(RangerDto rangerDto)
        {
            var user = new ApplicationUser
            {
                UserName = rangerDto.Email,
                Email = rangerDto.Email,
                EmailConfirmed = true, // TODO: implement email confirmations?
                RangerId = rangerDto.Id
            };
            var result = await _authenticationService.RegisterUserAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest("User registration failed.");
            }

            var roleResult = await _authorizationService.AssignRoleAsync(user, "Ranger");

            if (roleResult == null || !roleResult.Succeeded)
            {
                return BadRequest("Failed to assign role ranger to user.");
            }

            return Ok($"Successfully registered user {rangerDto.Email}.");
        }

        /// <summary>
        /// Admin user can assign roles to users.
        /// </summary>
        /// <param name="userEmail">User being assigned the role.</param>
        /// <param name="role">The role being assigned.</param>
        /// <returns>Status code 200 Ok, 404 Not found if no user with said email exists, 400 BaddRequest if assinging role was not possible.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("assign-role/{userEmail}/{role}")]
        public async Task<ActionResult> AssignRole(string userEmail, string role)
        {
            var user = await _authenticationService.GetUserAsync(userEmail);
            if (user == null)
            {
                return NotFound($"User with email {userEmail} was not found.");
            }
            var result = await _authorizationService.AssignRoleAsync(user, role);


            if (result == null)
            {
                return BadRequest("Assigning role was not succesfull");
            }

            if (result.Succeeded)
            {
                return Ok($"Successfully assigned role {role} to {userEmail}");
            }

            return BadRequest(result.ToString());
        }

        /// <summary>
        /// Attempts to sign in a user.
        /// </summary>
        /// <param name="request">SigninRequest - email and password.</param>
        /// <returns>Status code 200 and a UserDto of the signed in user, 
        /// or 401 Unauthorized if wrong email/password,
        /// or 400 BadRequest if user with said email was not found.</returns>
        [HttpPost("signin")]
        public async Task<ActionResult<UserDto>> SignIn([FromBody] SignInRequest request)
        {
            var result = await _authenticationService.SignInAsync(request.Email, request.Password);

            if (result.Succeeded)
            {
                var user = await _authenticationService.GetUserAsync(request.Email);
                if (user == null)
                {
                    return BadRequest($"Internal Error occured, user {request.Email} not found.");
                }
                string role = await _authorizationService.GetRoleAsync(user);
                if (role == "")
                {
                    return StatusCode(500, "Internal Error occured, no role is assigned to user");
                }
                return Ok(new UserDto { Email = request.Email, RangerId = user.RangerId, Role = role });
            }

            return Unauthorized("Nesprávný email nebo heslo.");
        }
        /// <summary>
        /// Sign out user.
        /// </summary>
        /// <returns>Status code 200 Ok.</returns>
        [Authorize]
        [HttpPost("signout")]
        public async Task<ActionResult> SignOutt()
        {
            await _authenticationService.SignOutAsync();
            return Ok("Sign-out successful");
        }

        /// <summary>
        /// Gets the currently signed in user.
        /// </summary>
        /// <returns>Status code 200 Ok and signed in user,
        /// or 404 NotFound if no user was signed in,
        /// or 500 If user was signed in, but without a role.</returns>
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _authenticationService.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("No user is signed in.");
            }
            string role = await _authorizationService.GetRoleAsync(user);
            if (role == "")
            {
                return StatusCode(500, "Internal Error occured, no role is assigned to user");
            }
            return Ok(new UserDto { Email = user.Email ?? "", RangerId = user.RangerId, Role = role });
        }

        // Future Todo: setting (link), changing passwords.

    }
}
