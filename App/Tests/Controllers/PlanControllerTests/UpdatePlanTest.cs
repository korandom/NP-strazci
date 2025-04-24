
using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using App.Server.Services.Authentication;
using App.Server.Services.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers.PlanControllerTests
{
    public class UpdatePlanTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAppAuthenticationService> _mockAuthService;
        private readonly Mock<IAppAuthorizationService> _mockAuthorizationService;
        private readonly Mock<IGenericRepository<Plan>> _mockPlanRepository;
        private readonly Mock<IGenericRepository<Route>> _mockRouteRepository;
        private readonly Mock<IGenericRepository<Vehicle>> _mockVehicleRepository;
        private readonly PlanController _controller;

        public UpdatePlanTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAuthService = new Mock<IAppAuthenticationService>();
            _mockAuthorizationService = new Mock<IAppAuthorizationService>();
            _mockPlanRepository = new Mock<IGenericRepository<Plan>>();
            _mockRouteRepository = new Mock<IGenericRepository<Route>>();
            _mockVehicleRepository = new Mock<IGenericRepository<Vehicle>>();

            _mockUnitOfWork.Setup(u => u.PlanRepository).Returns(_mockPlanRepository.Object);
            _mockUnitOfWork.Setup(u => u.RouteRepository).Returns(_mockRouteRepository.Object);
            _mockUnitOfWork.Setup(u => u.VehicleRepository).Returns(_mockVehicleRepository.Object);

            _controller = new PlanController(
                _mockUnitOfWork.Object,
                _mockAuthService.Object,
                _mockAuthorizationService.Object,
                null!
            );
        }

        [Fact]
        public async Task UpdatePlanSuccess()
        {
            // arrange
            var date = new DateOnly(2025, 1, 1);
            var ranger = new Ranger { Id = 1, Email = "abc@gmail.com", FirstName = "a", LastName = "b" };
            var plan = new Plan(date, ranger);
            var planDto = new PlanDto (date,ranger.ToDto(), [1], [1] );

            _mockPlanRepository.Setup(p => p.GetById(date, ranger.Id)).ReturnsAsync(plan);
            _mockRouteRepository.Setup(r => r.Get(r => planDto.RouteIds.Contains(r.Id), "")).ReturnsAsync(new List<Route> { new Route { Id = 1 } });
            _mockVehicleRepository.Setup(v => v.Get(v => planDto.VehicleIds.Contains(v.Id), "")).ReturnsAsync(new List<Vehicle> { new Vehicle { Id = 1 } });

            // act
            var result = await _controller.UpdatePlan(planDto);

            // assert
            var okResult = Assert.IsType<OkResult>(result);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task RangerDoesNotExist()
        {
            //arrange
            var date = new DateOnly(2025, 1, 1);
            var ranger = new Ranger { Id = 1, Email = "abc@gmail.com", FirstName = "a", LastName = "b" };
            var plan = new Plan(date, ranger);
            var planDto = new PlanDto(date, ranger.ToDto(), [1], [1]);


            _mockPlanRepository.Setup(p => p.GetById(date, ranger.Id)).ReturnsAsync((Plan?)null);
            _mockUnitOfWork.Setup(u => u.RangerRepository.GetById(ranger.Id)).ReturnsAsync((Ranger?)null);

            // act
            var result = await _controller.UpdatePlan(planDto);

            // assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Ranger id not found.", badRequest.Value);
        }

        [Fact]
        public async Task UnexpectedError()
        {
            // Arrange
            var date = new DateOnly(2025, 1, 1);
            var ranger = new Ranger { Id = 1, Email = "abc@gmail.com", FirstName = "a", LastName = "b" };
            var plan = new Plan(date, ranger);
            var planDto = new PlanDto(date, ranger.ToDto(), [1], [1]);

            _mockPlanRepository.Setup(p => p.GetById(date, ranger.Id)).Throws(new Exception("Unexpected"));

            // Act
            var result = await _controller.UpdatePlan(planDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("An error occurred while updating the plan.", objectResult.Value);
        }

    }
}
