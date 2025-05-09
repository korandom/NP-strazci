﻿using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq.Expressions;

namespace Tests.Controllers.VehicleControllerTests
{
    public class GetVehiclesInDistrictTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Vehicle>> _mockVehicleRepo;
        private readonly Mock<IGenericRepository<District>> _mockDistrictRepo;
        private readonly VehicleController _controller;

        public GetVehiclesInDistrictTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockDistrictRepo = new Mock<IGenericRepository<District>>();
            _mockVehicleRepo = new Mock<IGenericRepository<Vehicle>>();
            _mockUnitOfWork.Setup(u => u.VehicleRepository).Returns(_mockVehicleRepo.Object);
            _mockUnitOfWork.Setup(u => u.DistrictRepository).Returns(_mockDistrictRepo.Object);

            _controller = new VehicleController(
                _mockUnitOfWork.Object
            );
        }

        [Fact]
        public async Task FailedToGetVehicles()
        {
            // arrange
            var districtId = 1;
            _mockVehicleRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Vehicle, bool>>>(), "")).ReturnsAsync((IEnumerable<Vehicle>?)null!);

            // act
            var result = await _controller.GetVehiclesInDistrict(districtId);

            // assert
            var badReq = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("District not found.", badReq.Value);
        }

        [Fact]
        public async Task GetVehiclesSuccess()
        {
            // arrange
            var districtId = 1;
            var vehicle = new Vehicle { DistrictId = 1, Id = 1, Type = "car", Name = "skoda" };

            _mockDistrictRepo.Setup(r => r.GetById(districtId)).ReturnsAsync(new District { Id = districtId, Name = "district" });
            _mockVehicleRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Vehicle, bool>>>(), "")).ReturnsAsync([vehicle]);

            // act
            var result = await _controller.GetVehiclesInDistrict(districtId);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedVehicles = okResult.Value as List<VehicleDto>;
            Assert.NotNull(returnedVehicles);
            Assert.Single(returnedVehicles);
        }
    }
}
