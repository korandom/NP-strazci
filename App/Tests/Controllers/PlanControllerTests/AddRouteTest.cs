
using App.Server.Controllers;
using App.Server.CSP;
using App.Server.DTOs;
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
    public class AddRouteTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAppAuthenticationService> _mockAuthService;
        private readonly Mock<IAppAuthorizationService> _mockAuthorizationService;
        private readonly PlanController _controller;

        public AddRouteTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAuthService = new Mock<IAppAuthenticationService>();
            _mockAuthorizationService = new Mock<IAppAuthorizationService>();

            _controller = new PlanController(
                _mockUnitOfWork.Object,
                _mockAuthService.Object,
                _mockAuthorizationService.Object,
                new RoutePlanGenerator()
            );
        }

        [Fact]
        public async Task UserUnauthorized()
        {
            // arrange
            int rangerId = 1;
            int routeId = 10;
            DateOnly date = new DateOnly(2025, 1, 1);

            _mockAuthService.Setup(s => s.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((ApplicationUser?)null);

            // act
            var result = await _controller.AddRoute(date, rangerId, routeId);

            // assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("User is not authorized to change this plan.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task RangerNotOwner()
        {
            // arrange
            int rangerId = 1;
            int routeId = 10;
            DateOnly date = new DateOnly(2025, 1, 1);
            var user = new ApplicationUser { RangerId = 2, Id = "3", Email = "abc@gmail.com" };

            _mockAuthService.Setup(s => s.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockAuthorizationService.Setup(a => a.IsUserOwner(user, rangerId)).Returns(false);
            // act
            var result = await _controller.AddRoute(date, rangerId, routeId);

            // assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("User is not authorized to change this plan.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task RouteDoesNotExist()
        {
            // arrange
            int rangerId = 1;
            int routeId = 10;
            DateOnly date = new DateOnly(2025, 1, 1);
            var user = new ApplicationUser();
            var ranger = new Ranger { Id = 1, Email = "abc@gmail.com", FirstName = "a", LastName = "b" };
            var plan = new Plan(date, ranger);
            _mockAuthService.Setup(s => s.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                            .ReturnsAsync(user);
            _mockAuthorizationService.Setup(a => a.IsUserOwner(user, rangerId)).Returns(true);
            _mockUnitOfWork.Setup(u => u.PlanRepository.GetById(date, rangerId)).ReturnsAsync(plan);
            _mockUnitOfWork.Setup(u => u.RouteRepository.GetById(routeId)).ReturnsAsync((Route?)null);

            // act
            var result = await _controller.AddRoute(date, rangerId, routeId);

            // assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Route id not found.", badRequest.Value);
        }

        [Fact]
        public async Task CreatingNewPlan()
        {
            // arrange
            int rangerId = 1;
            int routeId = 10;
            DateOnly date = new DateOnly(2025, 1, 1);
            var user = new ApplicationUser();
            var ranger = new Ranger { Id = 1, Email = "abc@gmail.com", FirstName = "a", LastName = "b" };
            var route = new App.Server.Models.AppData.Route { Id = 1, DistrictId = 1, Name = "a", Priority = 2 };

            _mockAuthService.Setup(s => s.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockAuthorizationService.Setup(a => a.IsUserOwner(user, rangerId)).Returns(true);
            _mockUnitOfWork.Setup(u => u.PlanRepository.GetById(date, rangerId)).ReturnsAsync((Plan?)null);

            _mockUnitOfWork.Setup(u => u.RangerRepository.GetById(ranger.Id)).ReturnsAsync(ranger);
            _mockUnitOfWork.Setup(u => u.RouteRepository.GetById(routeId)).ReturnsAsync(route);

            // act
            var result = await _controller.AddRoute(date, rangerId, routeId);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<PlanDto>(okResult.Value);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Exactly(2)); // once when creating, then updating
        }

        [Fact]
        public async Task AddRouteSuccess()
        {
            // arrange
            var appUser = new ApplicationUser();
            var date = new DateOnly(2025, 1, 1);
            var ranger = new Ranger { Id = 1, Email = "abc@gmail.com", FirstName = "a", LastName = "b" };
            var plan = new Plan(date, ranger);
            var route = new App.Server.Models.AppData.Route { Id = 1, DistrictId = 1, Name = "a", Priority = 2 };

            _mockAuthService.Setup(s => s.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(appUser);
            _mockAuthorizationService.Setup(a => a.IsUserOwner(appUser, 1)).Returns(true);
            _mockUnitOfWork.Setup(u => u.PlanRepository.GetById(It.IsAny<DateOnly>(), 1)).ReturnsAsync(plan);
            _mockUnitOfWork.Setup(u => u.RouteRepository.GetById(1)).ReturnsAsync(route);

            // act
            var result = await _controller.AddRoute(date, 1, 1);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Exactly(1));
        }
    }
}
