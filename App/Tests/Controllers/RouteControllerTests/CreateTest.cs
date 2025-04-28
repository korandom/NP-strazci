using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers.RouteControllerTests
{
    public class CreateTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Route>> _mockRouteRepo;
        private readonly Mock<IGenericRepository<District>> _mockDistrictRepo;

        private readonly RouteController _controller;

        public CreateTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRouteRepo = new Mock<IGenericRepository<Route>>();
            _mockDistrictRepo = new Mock<IGenericRepository<District>>();
            _mockUnitOfWork.Setup(u => u.RouteRepository).Returns(_mockRouteRepo.Object);
            _mockUnitOfWork.Setup(u => u.DistrictRepository).Returns(_mockDistrictRepo.Object);


            _controller = new RouteController(
                _mockUnitOfWork.Object
            );
        }

        [Fact]
        public async Task DistrictDoesNotExist()
        {
            // arrange
            var districtId = 1;
            var routeDto = new RouteDto(2, "route", 2, null, districtId);
            _mockDistrictRepo.Setup(r => r.GetById(districtId)).ReturnsAsync((District?)null);

            // act
            var result = await _controller.Create(routeDto);

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
            var routeDto = new RouteDto(2, "route", 2, null, districtId);
            _mockDistrictRepo.Setup(r => r.GetById(districtId)).ReturnsAsync(district);

            // act
            var result = await _controller.Create(routeDto);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<RouteDto>(okResult.Value);
            _mockRouteRepo.Verify(r => r.Add(It.IsAny<Route>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
