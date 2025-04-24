using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers.VehicleControllerTests
{
    public class DeleteTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Vehicle>> _mockVehicleRepo;
        
        private readonly VehicleController _controller;

        public DeleteTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockVehicleRepo = new Mock<IGenericRepository<Vehicle>>();
            _mockUnitOfWork.Setup(u => u.VehicleRepository).Returns(_mockVehicleRepo.Object);


            _controller = new VehicleController(
                _mockUnitOfWork.Object
            );
        }

        [Fact]
        public async Task VehicleDoesNotExist()
        {
            // arrange
            var vehicleId = 1;
            var vehicleDto = new VehicleDto { DistrictId= 1, Id = vehicleId, Name = "skoda", Type = "car" };

            _mockVehicleRepo.Setup(r => r.GetById(vehicleId)).ReturnsAsync((Vehicle?)null);

            // act
            var result = await _controller.Delete(vehicleId);

            // assert
            var badReq = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Vehicle id not found.", badReq.Value);
        }

        [Fact]
        public async Task DeleteSucess()
        {
            // arrange
            var vehicleId = 1;
            var vehicle = new Vehicle { DistrictId = 1, Id = vehicleId, Type = "car", Name = "skoda" };
            _mockVehicleRepo.Setup(r => r.GetById(vehicleId)).ReturnsAsync(vehicle);
            

            // act
            var result = await _controller.Delete(vehicleId);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockVehicleRepo.Verify(r => r.Delete(It.IsAny<Vehicle>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
