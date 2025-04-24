using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.Identity;
using App.Server.Services.Authentication;
using App.Server.Services.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers.UserControllerTests
{
    public class RegisterRangerUserTest
    {
        private readonly Mock<IAppAuthenticationService> _mockAuthenticationService;
        private readonly Mock<IAppAuthorizationService> _mockAuthorizationService;
        private readonly UserController _controller;

        public RegisterRangerUserTest()
        {
            _mockAuthenticationService = new Mock<IAppAuthenticationService>();
            _mockAuthorizationService = new Mock<IAppAuthorizationService>();
            _controller = new UserController(_mockAuthenticationService.Object, _mockAuthorizationService.Object);
        }
        [Fact]
        public async Task RegisteringFailed()
        {
            //arrange
            string email = "ranger@gmail.com";
            var rangerDto = new RangerDto { Id = 1, Email = email };
            var failedResult = IdentityResult.Failed(new IdentityError
            {
                Code = "Failed",
                Description = "Test Failure."
            });

            _mockAuthenticationService.Setup(s => s.RegisterUserAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(failedResult);
            //act
            var result = await _controller.RegisterRangerUser(rangerDto);

            //assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task MakingNewUserARangerFailed()
        {
            //arrange
            string email = "ranger@gmail.com";
            var rangerDto = new RangerDto { Id = 1, Email = email };
            var failedResult = IdentityResult.Failed(new IdentityError
            {
                Code = "Failed",
                Description = "Test Failure."
            });

            _mockAuthenticationService.Setup(s => s.RegisterUserAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            _mockAuthorizationService.Setup(s => s.AssignRoleAsync(It.IsAny<ApplicationUser>(), "Ranger")).ReturnsAsync(failedResult);

            // act
            var result = await _controller.RegisterRangerUser(rangerDto);

            // assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RegisterRangerUserSuccess()
        {
            // arrange
            string email = "ranger@gmail.com";
            var rangerDto = new RangerDto { Id = 1, Email = email };
            var user = new ApplicationUser { Email = email, UserName = "ranger", RangerId = 2 };

            _mockAuthenticationService.Setup(s => s.RegisterUserAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            _mockAuthorizationService.Setup(s => s.AssignRoleAsync(It.IsAny<ApplicationUser>(), "Ranger")).ReturnsAsync(IdentityResult.Success);

            //act
            var result = await _controller.RegisterRangerUser(rangerDto);

            //assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
