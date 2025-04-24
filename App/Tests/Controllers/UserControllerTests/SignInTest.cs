using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.Identity;
using App.Server.Services.Authentication;
using App.Server.Services.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers.UserControllerTests
{
    public class SignInTest
    {
        private readonly Mock<IAppAuthenticationService> _mockAuthenticationService;
        private readonly Mock<IAppAuthorizationService> _mockAuthorizationService;
        private readonly UserController _controller;

        public SignInTest()
        {
            _mockAuthenticationService = new Mock<IAppAuthenticationService>();
            _mockAuthorizationService = new Mock<IAppAuthorizationService>();
            _controller = new UserController(_mockAuthenticationService.Object, _mockAuthorizationService.Object);
        }

        [Fact]
        public async Task SignInSuccess()
        {
            // arrange
            var request = new SignInRequest { Email = "user@test.com", Password = "pass" };
            var user = new ApplicationUser { Email = request.Email, RangerId = 1 };

            _mockAuthenticationService.Setup(s => s.SignInAsync(request.Email, request.Password)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _mockAuthenticationService.Setup(s => s.GetUserAsync(request.Email)).ReturnsAsync(user);
            _mockAuthorizationService.Setup(s => s.GetRoleAsync(user)).ReturnsAsync("Ranger");
            
            //act
            var result = await _controller.SignIn(request);

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            _mockAuthenticationService.Verify(service => service.SignInAsync(request.Email, request.Password), Times.Once);
            var userDto = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(request.Email, userDto.Email);
        }

        [Fact]
        public async Task SignOuttSuccess()
        {
            //act
            var result = await _controller.SignOutt();
            
            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockAuthenticationService.Verify(service => service.SignOutAsync(), Times.Once);
            Assert.Equal("Sign-out successful", okResult.Value);
        }
    }
}
