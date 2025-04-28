using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers.RouteControllerTests
{
    public class DeleteTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Route>> _mockRouteRepo;

        private readonly RouteController _controller;

        public DeleteTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRouteRepo = new Mock<IGenericRepository<Route>>();
            _mockUnitOfWork.Setup(u => u.RouteRepository).Returns(_mockRouteRepo.Object);


            _controller = new RouteController(
                _mockUnitOfWork.Object
            );
        }

        [Fact]
        public async Task RouteDoesNotExist()
        {
            // arrange
            var routeId = 1;
            var routeDto = new RouteDto(routeId, "route", 2, null, 1);

            _mockRouteRepo.Setup(r => r.GetById(routeId)).ReturnsAsync((Route?)null);

            // act
            var result = await _controller.Delete(routeId);

            // assert
            var badReq = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Route id not found.", badReq.Value);
        }

        [Fact]
        public async Task DeleteSucess()
        {
            // arrange
            var routeId = 1;
            var route = new App.Server.Models.AppData.Route { Id = routeId, DistrictId = 1, Name = "route", Priority = 2, ControlPlace = null };

            _mockRouteRepo.Setup(r => r.GetById(routeId)).ReturnsAsync(route);


            // act
            var result = await _controller.Delete(routeId);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockRouteRepo.Verify(r => r.Delete(It.IsAny<Route>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
