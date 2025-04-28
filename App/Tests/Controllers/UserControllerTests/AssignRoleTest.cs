using App.Server.Controllers;
using App.Server.Models.Identity;
using App.Server.Services.Authentication;
using App.Server.Services.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers.UserControllerTests
{
    public class AssignRoleTest
    {
        private readonly Mock<IAppAuthenticationService> _mockAuthenticationService;
        private readonly Mock<IAppAuthorizationService> _mockAuthorizationService;
        private readonly UserController _controller;

        public AssignRoleTest()
        {
            _mockAuthenticationService = new Mock<IAppAuthenticationService>();
            _mockAuthorizationService = new Mock<IAppAuthorizationService>();
            _controller = new UserController(_mockAuthenticationService.Object, _mockAuthorizationService.Object);
        }

        [Fact]
        public async Task UserDoesNotExist()
        {
            //arrange
            string email = "head@gmail.com";
            _mockAuthenticationService.Setup(s => s.GetUserAsync(email)).ReturnsAsync((ApplicationUser?)null);

            //act
            var result = await _controller.AssignRole(email, "HeadOfDistrict");

            //assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("not found", notFound.Value.ToString());
        }

        [Fact]
        public async Task AssingRoleFailed()
        {
            // arrange
            string email = "head@gmail.com";
            var user = new ApplicationUser { Email = email, RangerId = 1 };
            var failedResult = IdentityResult.Failed(new IdentityError
            {
                Code = "Failed",
                Description = "Test Failure."
            });
            _mockAuthenticationService.Setup(s => s.GetUserAsync(email)).ReturnsAsync(user);
            _mockAuthorizationService.Setup(s => s.AssignRoleAsync(user, "HeadOfDistrict")).ReturnsAsync(failedResult);

            //act
            var result = await _controller.AssignRole(user.Email, "HeadOfDistrict");

            // assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AssingRoleSuccess()
        {
            //arrange
            string email = "head@gmail.com";
            var user = new ApplicationUser { Email = email, RangerId = 1 };
            _mockAuthenticationService.Setup(s => s.GetUserAsync(email)).ReturnsAsync(user);
            _mockAuthorizationService.Setup(s => s.AssignRoleAsync(user, "HeadOfDistrict")).ReturnsAsync(IdentityResult.Success);

            //act
            var result = await _controller.AssignRole(user.Email, "HeadOfDistrict");

            //assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
