
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
    public class RemoveVehicleTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAppAuthenticationService> _mockAuthService;
        private readonly Mock<IAppAuthorizationService> _mockAuthorizationService;
        private readonly Mock<IGenericRepository<Plan>> _mockPlanRepository;
        private readonly Mock<IGenericRepository<Vehicle>> _mockVehicleRepository;

        private readonly PlanController _controller;

        public RemoveVehicleTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAuthService = new Mock<IAppAuthenticationService>();
            _mockAuthorizationService = new Mock<IAppAuthorizationService>();
            _mockPlanRepository = new Mock<IGenericRepository<Plan>>();
            _mockVehicleRepository = new Mock<IGenericRepository<Vehicle>>();

            _mockUnitOfWork.Setup(u => u.PlanRepository).Returns(_mockPlanRepository.Object);
            _mockUnitOfWork.Setup(u => u.VehicleRepository).Returns(_mockVehicleRepository.Object);

            _controller = new PlanController(
                _mockUnitOfWork.Object,
                _mockAuthService.Object,
                _mockAuthorizationService.Object,
                null!
            );
        }
        [Fact]
        public async Task PlanDoesNotExist()
        {
            // arrange
            var date = new DateOnly(2025, 1, 1);
            int rangerId = 1, vehicleId = 10;

            _mockPlanRepository.Setup(r => r.GetById(date, rangerId)).ReturnsAsync((Plan?)null);

            // act
            var result = await _controller.RemoveVehicle(date, rangerId, vehicleId);

            // assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Plan not found.", notFound.Value);
        }

        [Fact]
        public async Task VehicleDoesNotExist()
        {
            // arrange
            var date = new DateOnly(2025, 1, 1);
            int rangerId = 1;
            int vehicleId = 10;
            var ranger = new Ranger { Id = rangerId, Email = "abc@gmail.com", FirstName = "a", LastName = "b" };
            var plan = new Plan(date, ranger);

            _mockPlanRepository.Setup(r => r.GetById(date, rangerId)).ReturnsAsync(plan);
            _mockVehicleRepository.Setup(v => v.GetById(vehicleId)).ReturnsAsync((Vehicle?)null);

            // act
            var result = await _controller.RemoveVehicle(date, rangerId, vehicleId);

            // assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Vehicle id not found.", badRequest.Value);
        }

        [Fact]
        public async Task VehicleRemovedAndPlanEmpty()
        {
            // arrange
            var date = new DateOnly(2025, 1, 1);
            int rangerId = 1, vehicleId = 10;

            var ranger = new Ranger { Id = 1, Email = "abc@gmail.com", FirstName = "a", LastName = "b" };
            var vehicle = new Vehicle { Id = vehicleId };
            var plan = new Plan(date, ranger)
            {
                Routes = new List<Route>(),
                Vehicles = new List<Vehicle> { vehicle }
            };
            vehicle.Plans.Add(plan);

            _mockPlanRepository.Setup(p => p.GetById(date, rangerId)).ReturnsAsync(plan);
            _mockVehicleRepository.Setup(v => v.GetById(vehicleId)).ReturnsAsync(vehicle);

            // act
            var result = await _controller.RemoveVehicle(date, rangerId, vehicleId);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<PlanDto>(okResult.Value);
            _mockPlanRepository.Verify(p => p.Delete(plan), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveVehicle_PlanNotDeleted_ReturnsOk()
        {
            // arrange
            var date = new DateOnly(2025, 1, 1);
            int rangerId = 1, vehicleId = 10;

            var ranger = new Ranger { Id = rangerId };
            var vehicle = new Vehicle { Id = vehicleId };
            var plan = new Plan(date, ranger)
            {
                Routes = new List<Route> { new Route() },
                Vehicles = new List<Vehicle> { vehicle }
            };
            vehicle.Plans.Add(plan);

            _mockPlanRepository.Setup(p => p.GetById(date, rangerId)).ReturnsAsync(plan);
            _mockVehicleRepository.Setup(v => v.GetById(vehicleId)).ReturnsAsync(vehicle);

            // act
            var result = await _controller.RemoveVehicle(date, rangerId, vehicleId);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<PlanDto>(okResult.Value);
            _mockPlanRepository.Verify(p => p.Delete(It.IsAny<Plan>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
