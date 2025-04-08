using App.Server.DTOs;
using App.Server.Models.Identity;
using App.Server.Services.Authentication;
using App.Server.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IAppAuthenticationService authenticationService, IAppAuthorizationService authorizationService) : ControllerBase
    {
        private readonly IAppAuthenticationService _authenticationService = authenticationService;
        private readonly IAppAuthorizationService _authorizationService = authorizationService;

        // Registering a new user connected to ranger
        [Authorize(Roles = "Admin,HeadOfDistrict")]
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

            if (result.Succeeded)
            {
                await _authorizationService.AssignRoleAsync(user, "Ranger");
            }

            return Ok($"Succesfully registered user {rangerDto.Email}.");
        }

        // Admin can assign roles to users
        [Authorize(Roles = "Admin")]
        [HttpPut("assign-role/{userEmail}")]
        public async Task<IActionResult> AssignRole(string userEmail, string role)
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

        // signin
        [HttpPost("signin")]
        public async Task<ActionResult<UserDto>> SignIn([FromBody] SignInRequest request)
        {
            var result = await _authenticationService.SignInAsync(request.Email, request.Password);

            if (result.Succeeded)
            {
                var user = await _authenticationService.GetUserAsync(request.Email);
                if (user == null)
                {
                    return StatusCode(500, $"Internal Error occured, user {request.Email} not found.");
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
        // signout
        [Authorize]
        [HttpPost("signout")]
        public async Task<IActionResult> SignOutt()
        {
            await _authenticationService.SignOutAsync();
            return Ok("Sign-out successful");
        }

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
            return Ok(new UserDto { Email = user.Email, RangerId = user.RangerId, Role = role });
        }
        // set password -- user created with admin and is setting up first passwrod via link with token and email ? 
        // change password

    }
}
