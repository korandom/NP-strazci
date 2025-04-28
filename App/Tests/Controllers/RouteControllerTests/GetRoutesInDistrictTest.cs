using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq.Expressions;

namespace Tests.Controllers.RouteControllerTests
{
    public class GetVehiclesInDistrictTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Route>> _mockRouteRepo;
        private readonly Mock<IGenericRepository<District>> _mockDistrictRepo;
        private readonly RouteController _controller;

        public GetVehiclesInDistrictTest()
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
        public async Task FailedToGetRoutes()
        {
            // arrange
            var districtId = 1;
            _mockRouteRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Route, bool>>>(), "")).ReturnsAsync((IEnumerable<Route>?)null!);

            // act
            var result = await _controller.GetRoutesInDistrict(districtId);

            // assert
            var badReq = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("District not found.", badReq.Value);
        }

        [Fact]
        public async Task GetRoutesSuccess()
        {
            // arrange
            var districtId = 1;
            var route = new App.Server.Models.AppData.Route { Id = 2, DistrictId = districtId, Name = "route", Priority = 2, ControlPlace = null };
            var route2 = new App.Server.Models.AppData.Route { Id = 3, DistrictId = districtId, Name = "route2", Priority = 2, ControlPlace = new ControlPlace { ControlPlaceDescription = "here", ControlTime = "14:00" } };
            _mockDistrictRepo.Setup(r => r.GetById(districtId)).ReturnsAsync(new District { Id = districtId, Name = "district" });
            _mockRouteRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Route, bool>>>(), "")).ReturnsAsync([route, route2]);

            // act
            var result = await _controller.GetRoutesInDistrict(districtId);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRoutes = okResult.Value as List<RouteDto>;
            Assert.NotNull(returnedRoutes);
            Assert.Equal(2, returnedRoutes.Count);
        }
    }
}
