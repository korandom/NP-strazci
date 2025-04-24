using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using App.Server.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers.RouteControllerTests
{
    public class UpdateTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Route>> _mockRouteRepo;
        private readonly RouteController _controller;

        public UpdateTest()
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
            var result = await _controller.Update(routeDto);

            // assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Route not found.", notFound.Value);
        }

        [Fact]
        public async Task UpdateSucess()
        {
            // arrange
            var routeId = 1;
            var route = new App.Server.Models.AppData.Route { Id = routeId, DistrictId = 1, Name = "route", Priority = 2, ControlPlace = null };
            var routeDto = new RouteDto(routeId, "new", 1, null, 1);
            _mockRouteRepo.Setup(r => r.GetById(routeId)).ReturnsAsync(route);


            // act
            var result = await _controller.Update(routeDto);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<RouteDto>(okResult.Value);
            _mockRouteRepo.Verify(r => r.Update(It.IsAny<Route>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}