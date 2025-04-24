

using App.Server.Controllers;
using App.Server.Models.AppData;
using App.Server.Models.Identity;
using App.Server.Repositories.Interfaces;
using App.Server.Services.Authentication;
using App.Server.Services.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Tests.Controllers.PlanControllerTests
{
    public class RemoveRouteTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAppAuthenticationService> _mockAuthService;
        private readonly Mock<IAppAuthorizationService> _mockAuthorizationService;
        private readonly PlanController _controller;

        public RemoveRouteTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAuthService = new Mock<IAppAuthenticationService>();
            _mockAuthorizationService = new Mock<IAppAuthorizationService>();

            _controller = new PlanController(
                _mockUnitOfWork.Object,
                _mockAuthService.Object,
                _mockAuthorizationService.Object,
                null!
            );
        }

        [Fact]
        public async Task UserNotAuthorized()
        {
            // arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            var date = new DateOnly(2025, 1, 1);
            _mockAuthService.Setup(s => s.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((ApplicationUser?)null);

            //act
            var result = await _controller.RemoveRoute(date, 1, 1);

            //assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("User is not authorized to change this plan.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task PlanDoesNotExist()
        {
            // arrange
            var user = new ApplicationUser { RangerId = 2, Id = "3", Email = "abc@gmail.com" };
            var date = new DateOnly(2025, 1, 1);

            _mockAuthService.Setup(s => s.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockAuthorizationService.Setup(a => a.IsUserOwner(user, 1)).Returns(false);
            _mockAuthorizationService.Setup(a => a.IsInRoleAsync(user, "HeadOfDistrict")).ReturnsAsync(true);

            _mockUnitOfWork.Setup(u => u.PlanRepository.GetById(date, 1)).ReturnsAsync((Plan?)null);

            //act
            var result = await _controller.RemoveRoute(date, 1, 1);

            // asssert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Plan not found.", badRequest.Value);
        }

        [Fact]
        public async Task RouteDoesNotExist()
        {
            //arrange
            var date = new DateOnly(2025, 1, 1);
            var user = new ApplicationUser { RangerId = 1, Id = "3", Email = "abc@gmail.com" };
            var ranger = new Ranger { Id = 2, Email = "abc@gmail.com", FirstName = "a", LastName = "b" };
            var plan = new Plan(date, ranger);

            _mockAuthService.Setup(s => s.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockAuthorizationService.Setup(a => a.IsUserOwner(user, 1)).Returns(false);
            _mockAuthorizationService.Setup(a => a.IsInRoleAsync(user, "HeadOfDistrict")).ReturnsAsync(true);

            _mockUnitOfWork.Setup(u => u.PlanRepository.GetById(date, 1)).ReturnsAsync(plan);
            _mockUnitOfWork.Setup(u => u.RouteRepository.GetById(1)).ReturnsAsync((App.Server.Models.AppData.Route?)null);

            //act
            var result = await _controller.RemoveRoute(date, 1, 1);

            //assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Route id not found.", badRequest.Value);
        }

        [Fact]
        public async Task RemoveRouteSuccess()
        {
            //arrange
            var date = new DateOnly(2025, 1, 1);
            var user = new ApplicationUser { RangerId = 1, Id = "3", Email = "abc@gmail.com" };
            var ranger = new Ranger { Id = 1, Email = "abc@gmail.com", FirstName = "a", LastName = "b" };
            var plan = new Plan(date, ranger);
            var route = new App.Server.Models.AppData.Route { Id = 1, DistrictId = 1, Name = "a", Priority = 2 };

            plan.Routes.Add(route);
            route.Plans.Add(plan);

            _mockAuthService.Setup(s => s.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockAuthorizationService.Setup(a => a.IsUserOwner(user, 1)).Returns(true);
            _mockUnitOfWork.Setup(u => u.PlanRepository.GetById(date, 1)).ReturnsAsync(plan);
            _mockUnitOfWork.Setup(u => u.RouteRepository.GetById(1)).ReturnsAsync(route);

            //act
            var result = await _controller.RemoveRoute(date, 1, 1);

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Exactly(1));
        }

    }
}
