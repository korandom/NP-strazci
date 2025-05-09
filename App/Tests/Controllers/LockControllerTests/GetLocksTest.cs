﻿using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers.LockControllerTests
{
    using LockModel = App.Server.Models.AppData.Lock;
    public class GetLocksTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<LockModel>> _mockLockRepository;
        private readonly Mock<IGenericRepository<District>> _mockDistrictRepository;

        private readonly LockController _lockController;

        public GetLocksTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLockRepository = new Mock<IGenericRepository<LockModel>>();
            _mockDistrictRepository = new Mock<IGenericRepository<District>>();
            _mockUnitOfWork.Setup(u => u.DistrictRepository).Returns(_mockDistrictRepository.Object);
            _mockUnitOfWork.Setup(u => u.LockRepository).Returns(_mockLockRepository.Object);
            _lockController = new LockController(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task NoLocks()
        {
            // arrange
            var date = new DateOnly(2025, 1, 1);
            var districtId = 1;
            IEnumerable<LockModel>? locks = [];
            _mockLockRepository.Setup(r => r.Get(l => l.DistrictId == districtId, "")).ReturnsAsync((IEnumerable<LockModel>?)null!);

            // act
            var result = await _lockController.GetLocks(districtId);

            // assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Locks not found.", badRequest.Value);
        }
        [Fact]
        public async Task EmptyLocks()
        {
            // arrange
            var districtId = 1;
            _mockLockRepository.Setup(r => r.Get(l => l.DistrictId == districtId, "")).ReturnsAsync([]);

            // act
            var result = await _lockController.GetLocks(districtId);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedLocks = okResult.Value as List<LockDto>;
            Assert.NotNull(returnedLocks);
            Assert.Empty(returnedLocks);
        }

        [Fact]
        public async Task GetSuccess()
        {
            // arrange
            var date = new DateOnly(2025, 1, 1);
            var districtId = 1;
            var lockIt = new App.Server.Models.AppData.Lock { Date = date, DistrictId = districtId };
            _mockLockRepository.Setup(r => r.Get(l => l.DistrictId == districtId, "")).ReturnsAsync([lockIt]);

            // act
            var result = await _lockController.GetLocks(districtId);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedLocks = okResult.Value as List<LockDto>;
            Assert.NotNull(returnedLocks);
            Assert.Single(returnedLocks);
        }
    }
}
