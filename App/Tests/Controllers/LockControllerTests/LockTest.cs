

using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers.LockControllerTests
{
    public class LockTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Lock>> _mockLockRepository;
        private readonly Mock<IGenericRepository<District>> _mockDistrictRepository;

        private readonly LockController _lockController;

        public LockTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLockRepository = new Mock<IGenericRepository<Lock>>();
            _mockDistrictRepository = new Mock<IGenericRepository<District>>();
            _mockUnitOfWork.Setup(u => u.DistrictRepository).Returns(_mockDistrictRepository.Object);
            _mockUnitOfWork.Setup(u => u.LockRepository).Returns(_mockLockRepository.Object);
            _lockController = new LockController(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task DistrictDoesNotExist()
        {
            // arrange
            var date = new DateOnly(2025, 1, 1);
            var districtId = 1;
            _mockDistrictRepository.Setup(r => r.GetById(districtId)).ReturnsAsync((District?)null);

            // act
            var result = await _lockController.Lock(date, districtId);

            // assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("District id not found.", badRequest.Value);
        }

        [Fact]
        public async Task LockedSuccess()
        {
            // arrange
            var date = new DateOnly(2025, 1, 1);
            var districtId = 1;
            var district = new District { Id = districtId, Name = "District" };
            _mockDistrictRepository.Setup(r => r.GetById(districtId)).ReturnsAsync(district);

            // act
            var result = await _lockController.Lock(date, districtId);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockLockRepository.Verify(r => r.Add(It.IsAny<Lock>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
