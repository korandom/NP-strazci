using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers.VehicleControllerTests
{
    public class CreateTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Vehicle>> _mockVehicleRepo;
        private readonly Mock<IGenericRepository<District>> _mockDistrictRepo;

        private readonly VehicleController _controller;

        public CreateTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockVehicleRepo = new Mock<IGenericRepository<Vehicle>>();
            _mockDistrictRepo = new Mock<IGenericRepository<District>>();
            _mockUnitOfWork.Setup(u => u.VehicleRepository).Returns(_mockVehicleRepo.Object);
            _mockUnitOfWork.Setup(u => u.DistrictRepository).Returns(_mockDistrictRepo.Object);


            _controller = new VehicleController(
                _mockUnitOfWork.Object
            );
        }

        [Fact]
        public async Task DistrictDoesNotExist()
        {
            // arrange
            var districtId = 1;
            var vehicleDto = new VehicleDto { DistrictId = districtId, Id = 1, Name = "skoda", Type = "car" };
            _mockDistrictRepo.Setup(r => r.GetById(districtId)).ReturnsAsync((District?)null);

            // act
            var result = await _controller.Create(vehicleDto);

            // assert
            var badReq = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("District id not found.", badReq.Value);
        }

        [Fact]
        public async Task CreateSucess()
        {
            // arrange
            var districtId = 1;
            var district = new District { Id = districtId, Name = "District" };
            var vehicleDto = new VehicleDto { DistrictId = districtId, Id = 1, Name = "skoda", Type = "car" };
            _mockDistrictRepo.Setup(r => r.GetById(districtId)).ReturnsAsync(district);

            // act
            var result = await _controller.Create(vehicleDto);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<VehicleDto>(okResult.Value);
            _mockVehicleRepo.Verify(r => r.Add(It.IsAny<Vehicle>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
