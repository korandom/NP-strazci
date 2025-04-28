using App.Server.Controllers;
using App.Server.CSP;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using App.Server.Services.Authentication;
using App.Server.Services.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers.PlanControllerTests
{
    public class AddVehicleTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAppAuthenticationService> _mockAuthService;
        private readonly Mock<IAppAuthorizationService> _mockAuthorizationService;
        private readonly PlanController _controller;

        public AddVehicleTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAuthService = new Mock<IAppAuthenticationService>();
            _mockAuthorizationService = new Mock<IAppAuthorizationService>();
            var generator = new Mock<IRoutePlanGenerator>();

            _controller = new PlanController(
                _mockUnitOfWork.Object,
                _mockAuthService.Object,
                _mockAuthorizationService.Object,
                generator.Object
            );
        }


        [Fact]
        public async Task RangerDoesNotExist()
        {
            // arrange
            var date = new DateOnly(2025, 1, 1);
            int rangerId = 1;
            int vehicleId = 10;

            _mockUnitOfWork.Setup(u => u.PlanRepository.GetById(date, rangerId)).ReturnsAsync((Plan?)null);
            _mockUnitOfWork.Setup(u => u.RangerRepository.GetById(rangerId)).ReturnsAsync((Ranger?)null);

            // act
            var result = await _controller.AddVehicle(date, rangerId, vehicleId);

            // assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Ranger id not found.", badRequest.Value);
        }

        [Fact]
        public async Task VehicleDoesNotExist()
        {
            // arrange
            var date = new DateOnly(2025, 1, 1);
            int rangerId = 1;
            int vehicleId = 10;
            var ranger = new Ranger { Id = 1, Email = "abc@gmail.com", FirstName = "a", LastName = "b" };
            var plan = new Plan(date, ranger);

            _mockUnitOfWork.Setup(u => u.PlanRepository.GetById(date, rangerId)).ReturnsAsync(plan);
            _mockUnitOfWork.Setup(u => u.VehicleRepository.GetById(vehicleId)).ReturnsAsync((Vehicle?)null);

            // act
            var result = await _controller.AddVehicle(date, rangerId, vehicleId);

            // assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Vehicle id not found.", badRequest.Value);
        }

        [Fact]
        public async Task AddVehicleSuccess()
        {
            // arrange
            var date = new DateOnly(2025, 1, 1);
            int rangerId = 1;
            int vehicleId = 10;
            var ranger = new Ranger { Id = 1, Email = "abc@gmail.com", FirstName = "a", LastName = "b" };
            var plan = new Plan(date, ranger);
            var vehicle = new Vehicle { Id = vehicleId };

            _mockUnitOfWork.Setup(u => u.PlanRepository.GetById(date, rangerId)).ReturnsAsync(plan);
            _mockUnitOfWork.Setup(u => u.VehicleRepository.GetById(vehicleId)).ReturnsAsync(vehicle);

            // act
            var result = await _controller.AddVehicle(date, rangerId, vehicleId);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<PlanDto>(okResult.Value);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
