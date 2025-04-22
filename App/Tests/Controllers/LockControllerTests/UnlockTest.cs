
using App.Server.Controllers;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq.Expressions;

namespace Tests.Controllers.LockControllerTests
{
    public class UnlockTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Lock>> _mockLockRepository;
        private readonly Mock<IGenericRepository<District>> _mockDistrictRepository;

        private readonly LockController _lockController;

        public UnlockTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLockRepository = new Mock<IGenericRepository<Lock>>();
            _mockDistrictRepository = new Mock<IGenericRepository<District>>();
            _mockUnitOfWork.Setup(u => u.DistrictRepository).Returns(_mockDistrictRepository.Object);
            _mockUnitOfWork.Setup(u => u.LockRepository).Returns(_mockLockRepository.Object);
            _lockController = new LockController(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task LockDoesNotExist()
        {
            // arrange
            var date = new DateOnly(2025, 1, 1);
            var districtId = 1;
            _mockLockRepository.Setup(r => r.Get(l => (l.Date ==date && l.DistrictId == districtId), "")).ReturnsAsync([]);

            // act
            var result = await _lockController.Unlock(date, districtId);

            // assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Lock doesn't exist.", badRequest.Value);
        }

        [Fact]
        public async Task UnLockSuccess()
        {
            // arrange
            var date = new DateOnly(2025, 1, 1);
            var districtId = 1;
            var lockIt = new Lock { Date = date, DistrictId = districtId };
            _mockLockRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Lock, bool>>>(), "")).ReturnsAsync(new List<Lock> { lockIt });

            // act
            var result = await _lockController.Unlock(date, districtId);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockLockRepository.Verify(r => r.Delete(It.IsAny<Lock>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
