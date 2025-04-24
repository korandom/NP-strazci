
using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.Identity;
using App.Server.Services.Authentication;
using App.Server.Services.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Tests.Controllers.UserControllerTests
{
    public class GetCurrentUserTest
    {
        private readonly Mock<IAppAuthenticationService> _mockAuthenticationService;
        private readonly Mock<IAppAuthorizationService> _mockAuthorizationService;
        private readonly UserController _controller;

        public GetCurrentUserTest()
        {
            _mockAuthenticationService = new Mock<IAppAuthenticationService>();
            _mockAuthorizationService = new Mock<IAppAuthorizationService>();
            _controller = new UserController(_mockAuthenticationService.Object, _mockAuthorizationService.Object);
        }


        [Fact]
        public async Task NoUserSignedIn()
        {
            //arrange
            _mockAuthenticationService.Setup(s => s.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((ApplicationUser?)null);

            //act
            var result = await _controller.GetCurrentUser();
            
            //assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task UserDoesNotHaveRole()
        {
            //arrange
            var user = new ApplicationUser { Email = "ranger@gmail.com", UserName = "ranger", RangerId = 2 };

            _mockAuthenticationService.Setup(s => s.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockAuthorizationService.Setup(s => s.GetRoleAsync(user)).ReturnsAsync("");

            //act
            var result = await _controller.GetCurrentUser();

            //assert
            var status = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, status.StatusCode);
        }


        [Fact]
        public async Task CurrentUserSuccess()
        {
            //arrange
            var user = new ApplicationUser { Email = "ranger@gmail.com", UserName = "ranger", RangerId = 2 };

            _mockAuthenticationService.Setup(s => s.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockAuthorizationService.Setup(s => s.GetRoleAsync(user)).ReturnsAsync("Ranger");

            //act
            var result = await _controller.GetCurrentUser();

            //assert
            Assert.IsType<OkObjectResult>(result.Result);
        }


    }
}
